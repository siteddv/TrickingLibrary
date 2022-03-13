using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrickingLibrary.API.Settings;

namespace TrickingLibrary.API.BackgroundServices.VideoEditing
{
    public class FileManagerLocal : IFileManager
    {
        private readonly IOptionsMonitor<FileSettings> _fileSettingsMonitor;
        private readonly IWebHostEnvironment _env;
        
        public const string TempPrefix = "temp_";
        public const string ConvertedPrefix = "c";
        public const string ThumbnailPrefix = "t";
        public const string ProfilePrefix = "p";
        
        public FileManagerLocal(IWebHostEnvironment env, IOptionsMonitor<FileSettings> fileSettingsMonitor)
        {
            _env = env;
            _fileSettingsMonitor = fileSettingsMonitor;
        }

        private string WorkingDirectory => _env.WebRootPath;
        
        public string GetFfmpegPath() => Path.Combine(_env.ContentRootPath, "ffmpeg", "ffmpeg.exe");
        

        public bool Temporary(string fileName)
        {
            return fileName.StartsWith(TempPrefix);
        }

        public string GetFileUrl(string fileName, FileType fileType)
        {
            var settings = _fileSettingsMonitor.CurrentValue;
            return fileType switch
            {
                FileType.Image => $"{settings.ImageUrl}/{fileName}",
                FileType.Video => $"{settings.VideoUrl}/{fileName}",
                _ => throw new ArgumentException(nameof(fileType)),
            };
        }

        public bool TemporaryFileExists(string fileName)
        {
            var path = TemporarySavePath(fileName);
            return File.Exists(path);
        }

        public void DeleteTemporaryFile(string fileName)
        {
            var path = TemporarySavePath(fileName);
            
            if (File.Exists(path))
                File.Delete(path);
        }

        public string GetSavePath(string fileName) => 
            Path.Combine(WorkingDirectory, fileName);

        public async Task<string> SaveTemporaryFile(IFormFile video)
        {
            var fileName = string.Concat(TempPrefix, DateTime.Now.Ticks, Path.GetExtension(video.FileName));
            var savePath = TemporarySavePath(fileName);

            await using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            await video.CopyToAsync(fileStream);

            return fileName;
        }

        public string TemporarySavePath(string fileName)
        {
            return Path.Combine(WorkingDirectory, fileName);
        }
    }
}