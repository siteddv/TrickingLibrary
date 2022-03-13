using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrickingLibrary.API.Settings;

namespace TrickingLibrary.API.BackgroundServices.VideoEditing
{
    public static class RegisterService
    {
        public static IServiceCollection AddFileManager(this IServiceCollection services, IConfiguration config)
        {
            var settingsSection = config.GetSection(nameof(FileSettings));
            var settings = settingsSection.Get<FileSettings>();
            services.Configure<FileSettings>(settingsSection);

            if (settings.Provider.Equals(TrickingLibraryConstants.Files.Providers.Local))
            {
                services.AddSingleton<IFileManager, FileManagerLocal>();
            }
            else if (settings.Provider.Equals(TrickingLibraryConstants.Files.Providers.S3))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception($"Invalid File Manager Provider: {settings.Provider}");
            }

            return services;
        }
    }
}