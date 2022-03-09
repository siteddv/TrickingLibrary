﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrickingLibrary.Data;
using TrickingLibrary.Models;

namespace TrickingLibrary.API.BackgroundServices.VideoEditing
{
    public class VideoEditingBackgroundService : BackgroundService
    {
        private readonly ILogger<VideoEditingBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly VideoManager _videoManager;
        private readonly ChannelReader<EditVideoMessage> _channelReader;

        public VideoEditingBackgroundService(
            Channel<EditVideoMessage> channel,
            ILogger<VideoEditingBackgroundService> logger,
            IServiceProvider serviceProvider,
            VideoManager videoManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channelReader = channel.Reader;
            _videoManager = videoManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                var message = await _channelReader.ReadAsync(stoppingToken);
                var inputPath = _videoManager.TemporarySavePath(message.Input);
                var outputConvertedName = _videoManager.GenerateConvertedFileName();
                var outputThumbnailName = _videoManager.GenerateThumbnailFileName();
                var outputConvertedPath = _videoManager.TemporarySavePath(outputConvertedName);
                var outputThumbnailPath = _videoManager.TemporarySavePath(outputThumbnailName);
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = _videoManager.FfmpegPath,
                        Arguments = $"-y -i {inputPath} -an -vf scale=540x380 {outputConvertedPath} -ss 00:00:00 -vframes 1 -vf scale=540x380 {outputThumbnailPath}",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };

                    using (var process = new Process {StartInfo = startInfo})
                    {
                        process.Start();
                        process.WaitForExit();
                    }

                    if (!_videoManager.TemporaryFileExists(outputConvertedName))
                        throw new Exception("FFMPEG failed to generate converted video");
                    
                    if (!_videoManager.TemporaryFileExists(outputThumbnailName))
                        throw new Exception("FFMPEG failed to generate thumbnail");
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var submission = ctx.Submissions.FirstOrDefault(x => x.Id.Equals(message.SubmissionId));

                        submission.Video = new Video
                        {
                            VideoLink = outputConvertedName,
                            ThumbLink = outputThumbnailName,
                        };

                        await ctx.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Video Processing Failed for {0}", message.Input);
                    _videoManager.DeleteTemporaryFile(outputConvertedName);
                    _videoManager.DeleteTemporaryFile(outputThumbnailName);
                }
                finally
                {
                    _videoManager.DeleteTemporaryFile(message.Input);
                }
            }
        }
    }
}