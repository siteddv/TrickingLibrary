using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/tricks")]
    public class TricksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TricksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // /api/tricks
        [HttpGet]
        public IEnumerable<object> GetAll() => _dbContext
            .Tricks
            .Where(x => x.Active)
            .Select(TrickViewModel.Projection)
            .ToList();

        // /api/tricks/{id}
        [HttpGet("{id}")]
        public IActionResult  GetById(string id)
        { 
            var query = _dbContext.Tricks.AsQueryable();

            if (int.TryParse(id, out var intId))
            {
                query = query.Where(x => x.Id == intId);
            }
            else
            {
                query = query.Where(x => 
                    x.Slug.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                    && x.Active);
            }

            var trick = query
                .Select(TrickViewModel.Projection)
                .FirstOrDefault();

            if (trick == null)
                return NoContent();

            return Ok(trick);
        }

        // /api/tricks/{id}/submissions
        [HttpGet("{trickId}/submissions")]
        public IEnumerable<object> GetListSubmissionsForTrick(string trickId) =>
            _dbContext.Submissions
                .Include(x => x.User)
                .Where(x => x.TrickId.Equals(trickId, StringComparison.InvariantCultureIgnoreCase))
                .Select(SubmissionViewModel.Projection)
                .ToList<object>();

        // /api/tricks
        [HttpPost]
        public async Task<object> Create([FromBody] TrickForm trickForm)
        {
            var trick = new Trick
            {
                Slug  = trickForm.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = trickForm.Name,
                Version = 1,
                Description = trickForm.Description,
                Difficulty = trickForm.Difficulty,
                TrickCategories = trickForm.Categories.Select(x => new TrickCategory {CategoryId = x}).ToList()
            };
            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            _dbContext.Add(new ModerationItem
            {
                Target = trick.Id,
                Type = ModerationTypes.Trick,
            });
            await _dbContext.SaveChangesAsync();
            return TrickViewModel.Create(trick);
        }

        // /api/tricks
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TrickForm trickForm)
        {
            var trick = _dbContext.Tricks.FirstOrDefault(x => x.Id == trickForm.Id);
            
            if (trick == null)
                return NoContent();

            var newTrick = new Trick
            {
                Slug = trick.Slug,
                Name = trick.Name,
                Version = trick.Version + 1,
                Description = trickForm.Description,
                Difficulty = trickForm.Difficulty,
                Prerequisites = trickForm.Prerequisites
                    .Select(x => new TrickRelationship {PrerequisiteId = x})
                    .ToList(),
                Progressions = trickForm.Progressions
                    .Select(x => new TrickRelationship {ProgressionId = x})
                    .ToList(),
                TrickCategories = trickForm.Categories.Select(x => new TrickCategory {CategoryId = x}).ToList()
            };

            _dbContext.Add(newTrick);
            await _dbContext.SaveChangesAsync();
            _dbContext.Add(new ModerationItem
            {
                Current = trick.Id,
                Target = newTrick.Id,
                Type = ModerationTypes.Trick,
            });
            
            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            
            // todo redirect to the mod item instead of returning the trick
            return Ok(TrickViewModel.Create(newTrick));
        }

        // /api/tricks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var trick = _dbContext.Tricks.FirstOrDefault(x => x.Slug.Equals(id));
            
            if (trick == null)
                throw new ArgumentNullException(nameof(id), "Deleting trick by id is null");
            
            trick.Deleted = true;

            await _dbContext.SaveChangesAsync();
            
            return Ok();
        }
    }
}