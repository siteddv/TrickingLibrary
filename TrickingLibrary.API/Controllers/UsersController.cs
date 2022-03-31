﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TrickingLibrary.API.BackgroundServices.VideoEditing;
using TrickingLibrary.API.Settings;
using TrickingLibrary.API.ViewModels;
using TrickingLibrary.Data;
using TrickingLibrary.Models;

namespace TrickingLibrary.API.Controllers
{
    [Route("api/users")]
    [Authorize(TrickingLibraryConstants.Policies.User)]
    public class UsersController : ApiController
    {
        private readonly AppDbContext _ctx;

        public UsersController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (user != null) return Ok(user);

            user = new User
            {
                Id = userId,
                Username = Username
            };

            _ctx.Add(user);
            await _ctx.SaveChangesAsync();

            return Ok(user);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetUser(string id) => Ok();
        
        [HttpGet("{id}/submissions")]
        public Task<List<object>> GetUserSubmissions(string id)
        {
            return _ctx.Submissions
                .Include(x => x.Video)
                .Include(x => x.User)
                .Where(x => x.UserId.Equals(id))
                .Select(SubmissionViewModel.Projection)
                .ToListAsync<object>();
        }
        
        [HttpPut("me/image")]
        public async Task<IActionResult> UpdateProfileImage(
            IFormFile image,
            [FromServices] IFileManager fileManager)
        {
            if (image == null) return BadRequest();

            var userId = UserId;
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (user == null) return NoContent();

            var fileName = TrickingLibraryConstants.Files.GenerateProfileFileName();
            await using (var stream = System.IO.File.Create(fileManager.GetSavePath(fileName)))
            using (var imageProcessor = await Image.LoadAsync(image.OpenReadStream()))
            {
                imageProcessor.Mutate(x => x.Resize(48, 48));

                await imageProcessor.SaveAsync(stream, new JpegEncoder());
            }

            user.Image = fileManager.GetFileUrl(fileName, FileType.Image);
            await _ctx.SaveChangesAsync();
            return Ok(user);
        }
    }
}