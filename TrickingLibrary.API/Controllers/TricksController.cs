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
        public object GetById(string id) => 
            _dbContext.Tricks
                .Where(x => x.Active)
                .Where(x => x.Slug.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                .Select(TrickViewModel.Projection)
                .FirstOrDefault();

        // /api/tricks/{id}/submissions
        [HttpGet("{trickId}/submissions")]
        public IEnumerable<Submission> GetListSubmissionsForTrick(string trickId) => 
            _dbContext.Submissions
                .Include(x => x.Video)
                .Where(x => x.TrickId.Equals(trickId, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .AsEnumerable();

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
            _dbContext.Add(new ModerationItem
            {
                Target = trick.Slug,
                TargetVersion = trick.Version,
                Type = ModerationTypes.Trick,
            });
            await _dbContext.SaveChangesAsync();
            return TrickViewModel.Create(trick);
        }

        // /api/tricks
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TrickForm trickForm)
        {
            var trick = _dbContext.Tricks.FirstOrDefault(x => x.Slug == trickForm.Id);
            
            if (trick == null)
                return NoContent();

            var newTrick = new Trick
            {
                Slug = trick.Slug,
                Name = trick.Name,
                Version = _dbContext.Tricks.LatestVersion(1),
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
            _dbContext.Add(new ModerationItem
            {
                Target = trick.Slug,
                TargetVersion = newTrick.Version,
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