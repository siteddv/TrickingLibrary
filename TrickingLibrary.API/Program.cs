using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrickingLibrary.Data;
using TrickingLibrary.Models;

namespace TrickingLibrary.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                if (env.IsDevelopment())
                {
                    ctx.Difficulties.Add(new Difficulty {Id = "easy", Name = "Easy", Description = "Easy Test"});
                    ctx.Difficulties.Add(new Difficulty {Id = "hard", Name = "Hard", Description = "Hard Test"});
                    ctx.Categories.Add(new Category {Id = "kick", Name = "Kick", Description = "Kick Test"});
                    ctx.Categories.Add(new Category {Id = "flip", Name = "Flip", Description = "Flip Test"});
                    ctx.Categories.Add(new Category {Id = "transition", Name = "Transition", Description = "Transition Test"});
                    ctx.SaveChanges();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}