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
        [HttpGet("{id:int}")]
        public Trick GetById(int id) => _dbContext.Tricks.FirstOrDefault(x => x.Id == id);

        // /api/tricks/{id}/submissions
        [HttpGet("{trickId:int}")]
        public IEnumerable<Submission> GetListSubmissionsForTrick(int trickId) => 
            _dbContext.Submissions.Where(sub => sub.TrickId == trickId).AsEnumerable();

        // /api/tricks
        [HttpPost]
        public async Task<Trick> Create([FromBody] Trick trick)
        {
            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            
            return trick;
        }

        // /api/tricks
        [HttpPut]
        public async Task<Trick> Update([FromBody] Trick trick)
        {
            if (trick.Id == 0)
                return null;

            _dbContext.Add(trick);
            await _dbContext.SaveChangesAsync();
            return trick;
        }

        // /api/tricks/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}