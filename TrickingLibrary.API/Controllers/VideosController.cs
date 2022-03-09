using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrickingLibrary.API.BackgroundServices.VideoEditing;

namespace TrickingLibrary.API.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly VideoManager _videoManager;

        public VideosController(VideoManager videoManager)
        {
            _videoManager = videoManager;
        }

        [HttpGet("{videoName}")]
        public IActionResult GetVideo(string videoName)
        {
            var savePath = _videoManager.DevVideoPath(videoName);
            
            if (string.IsNullOrEmpty(savePath))
                return BadRequest();
            
            var fileStream = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "video/*");
        }
        
        [HttpPost]
        public async Task<string> UploadVideo(IFormFile video)
        {
            
            return await _videoManager.SaveTemporaryVideo(video);
        }
        
        [HttpDelete("{fileName}")]
        public IActionResult DeleteTemporaryVideo(string fileName)
        {
            if (!_videoManager.Temporary(fileName))
                return BadRequest();

            if (!_videoManager.TemporaryVideoExists(fileName))
                return NoContent();

            _videoManager.DeleteTemporaryVideo(fileName);

            return Ok();
        }
    }
}