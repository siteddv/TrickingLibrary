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
        public IEnumerable<object> GetAll() => _dbContext.Tricks.Select(TrickViewModel.Default).ToList();

        // /api/tricks/{id}
        [HttpGet("{id}")]
        public object GetById(string id) => 
            _dbContext.Tricks
                .Where(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                .Select(TrickViewModel.Default)
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
                Id = trickForm.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = trickForm.Name,
                Description = trickForm.Description,
                Difficulty = trickForm.Difficulty,
                TrickCategories = trickForm.Categories.Select(x => new TrickCategory {CategoryId = x}).ToList()
            };
            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            return TrickViewModel.Default.Compile().Invoke(trick);
        }

        // /api/tricks
        [HttpPut]
        public async Task<object> Update([FromBody] Trick trick)
        {
            if (string.IsNullOrEmpty(trick.Id))
                return null;

            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            return TrickViewModel.Default.Compile().Invoke(trick);
        }

        // /api/tricks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var trick = _dbContext.Tricks.FirstOrDefault(x => x.Id.Equals(id));
            
            if (trick == null)
                throw new ArgumentNullException(nameof(id), "Deleting trick by id is null");
            
            trick.Deleted = true;

            await _dbContext.SaveChangesAsync();
            
            return Ok();
        }
    }
}