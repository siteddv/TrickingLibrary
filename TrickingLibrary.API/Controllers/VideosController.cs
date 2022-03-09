using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrickingLibrary.API.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public VideosController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("{videoName}")]
        public IActionResult GetVideo(string videoName)
        {
            var savePath = Path.Combine(_env.WebRootPath, videoName);
            var fileStream = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "video/*");
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile video)
        {
            var mime = video.FileName.Split('.').Last();
            var fileName = string.Concat($"temp_{DateTime.Now.Ticks}", ".", mime);            
            var savePath = Path.Combine(_env.WebRootPath, fileName);

            await using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                await video.CopyToAsync(fileStream);
            }

            return Ok(fileName);
        }
    }
}