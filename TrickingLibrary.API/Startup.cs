using System.Threading.Channels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrickingLibrary.API.BackgroundServices;
using TrickingLibrary.Data;
using TrickingLibrary.Models;

namespace TrickingLibrary.API
{
    public class Startup
    {
        private const string AllCors = "All";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Dev"));
            
            services.AddHostedService<VideoEditingBackgroundService>();
            services.AddSingleton(_ => Channel.CreateUnbounded<EditVideoMessage>());
            
            services.AddCors(options =>
            {
                options.AddPolicy(AllCors, policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(AllCors);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}