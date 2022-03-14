﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrickingLibrary.API.Forms;
using TrickingLibrary.API.ViewModels;
using TrickingLibrary.Data;
using TrickingLibrary.Models;
using TrickingLibrary.Models.Moderation;

namespace TrickingLibrary.API.Controllers
{
    [ApiController]
    [Route("api/moderation-items")]
    public class ModerationItemController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ModerationItemController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IEnumerable<ModerationItem> All() => _ctx.ModerationItems
            .Where(x => !x.Deleted)
            .ToList();

        [HttpGet("{id:int}")]
        public object Get(int id) => _ctx.ModerationItems
            .Include(x => x.Comments)
            .Include(x => x.Reviews)
            .Where(x => x.Id.Equals(id))
            .Select(ModerationItemViewModel.Projection)
            .FirstOrDefault();
        
        [HttpGet("{id:int}/comments")]
        public IEnumerable<object> GetComments(int id) =>
            _ctx.Comments
                .Where(x => x.ModerationItemId.Equals(id))
                .Select(CommentViewModel.Projection)
                .ToList();
        
        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> Comment(int id, [FromBody] Comment comment)
        {
            if (!_ctx.ModerationItems.Any(x => x.Id == id))
                return NoContent();
            
            var regex = new Regex(@"\B(?<tag>@[a-zA-Z0-9-_]+)");

            comment.HtmlContent = regex.Matches(comment.Content)
                .Aggregate(comment.Content,
                    (content, match) =>
                    {
                        var tag = match.Groups["tag"].Value;
                        return content
                            .Replace(tag, $"<a href=\"{tag}-user-link\">{tag}</a>");
                    });

            comment.ModerationItemId = id;
            _ctx.Add(comment);
            await _ctx.SaveChangesAsync();

            return Ok(CommentViewModel.Create(comment));
        }
        
        [HttpGet("{id:int}/reviews")]
        public IEnumerable<Review> GetReviews(int id) =>
            _ctx.Reviews
                .Where(x => x.ModerationItemId.Equals(id))
                .ToList();

        [HttpPost("{id:int}/reviews")]
        public async Task<IActionResult> Review(int id, 
            [FromBody] ReviewForm reviewForm,
            [FromServices] VersionMigrationContext migrationContext)
        {
            var modItem = _ctx.ModerationItems
                .Include(x => x.Reviews)
                .FirstOrDefault(x => x.Id == id);
            
            if (modItem == null)
                return NoContent();

            if (modItem.Deleted)
                return BadRequest("Moderation item no longer exists.");
            
            // todo make this async safe
            var review = new Review
            {
                ModerationItemId = id,
                Comment = reviewForm.Comment,
                Status = reviewForm.Status,
            };
            
            _ctx.Add(review);
            
            // todo use configuration replace the magic '3'
            try
            {
                if (modItem.Reviews.Count >= 3)
                {
                    migrationContext.Migrate(modItem);
                    modItem.Deleted = true;
                }

                await _ctx.SaveChangesAsync();
            }
            catch (InvalidVersionException e)
            {
                return BadRequest(e.Message);
            }
            
            await _ctx.SaveChangesAsync();

            return Ok(ReviewViewModel.Create(review));
        }
    }
}