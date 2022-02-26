using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<Trick> GetAll() => _dbContext.Tricks.AsEnumerable();

        // /api/tricks/{id}
        [HttpGet("{id}")]
        public Trick GetById(string id) => 
            _dbContext.Tricks
                .FirstOrDefault(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));

        // /api/tricks/{id}/submissions
        [HttpGet("{trickId}/submissions")]
        public IEnumerable<Submission> GetListSubmissionsForTrick(string trickId) => 
            _dbContext.Submissions.Where(x => x.TrickId.Equals(trickId, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .AsEnumerable();

        // /api/tricks
        [HttpPost]
        public async Task<Trick> Create([FromBody] Trick trick)
        {
            trick.Id = trick.Name.Replace(" ", "-").ToLowerInvariant();
            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            
            return trick;
        }

        // /api/tricks
        [HttpPut]
        public async Task<Trick> Update([FromBody] Trick trick)
        {
            if (string.IsNullOrEmpty(trick.Id))
                return null;

            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            return trick;
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