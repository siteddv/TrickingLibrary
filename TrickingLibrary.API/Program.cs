using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrickingLibrary.Data;
using TrickingLibrary.Models;
using TrickingLibrary.Models.Moderation;

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
                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    var testUser = new IdentityUser("test"){Email = "test@test.com"};
                    userMgr.CreateAsync(testUser, "password").GetAwaiter().GetResult();

                    var mod = new IdentityUser("mod"){Email = "mod@test.com"};
                    userMgr.CreateAsync(mod, "password").GetAwaiter().GetResult();
                    userMgr.AddClaimAsync(mod,
                            new Claim(TrickingLibraryConstants.Claims.Role,
                                TrickingLibraryConstants.Roles.Mod))
                        .GetAwaiter()
                        .GetResult();
                    
                    ctx.Add(new Difficulty {Id = 1, Slug = "easy", Active = true, Version = 1, Name = "Easy", Description = "Easy Test"});
                    ctx.Add(new Difficulty {Id = 2, Slug = "medium", Active = true, Version = 1, Name = "Medium", Description = "Medium Test"});
                    ctx.Add(new Difficulty {Id = 3, Slug = "hard", Active = true, Version = 1, Name = "Hard", Description = "Hard Test"});
                    ctx.Add(new Category {Id = 1, Slug = "kick", Active = true, Version = 1, Name = "Kick", Description = "Kick Test"});
                    ctx.Add(new Category {Id = 2, Slug = "flip", Active = true, Version = 1, Name = "Flip", Description = "Flip Test"});
                    ctx.Add(new Category {Id = 3, Slug = "transition", Active = true, Version = 1, Name = "Transition", Description = "Transition Test"});
                    ctx.Add(new Trick
                    {
                        Id = 1,
                        Slug = "backwards-roll",
                        Name = "Backwards Roll",
                        Active = true,
                        Version = 1,
                        Description = "This is a test backwards roll",
                        Difficulty = "easy",
                        TrickCategories = new List<TrickCategory> {new TrickCategory {CategoryId = 2}}
                    });
                    ctx.Add(new Trick
                    {
                        Id = 2,
                        Slug = "forwards-roll",
                        Name = "Forwards Roll",
                        Active = true,
                        Version = 1,
                        Description = "This is a test forwards roll",
                        Difficulty = "easy",
                        TrickCategories = new List<TrickCategory> {new TrickCategory {CategoryId = 2}}
                    });
                    ctx.Add(new Trick
                    {
                        Id = 2,
                        Slug = "back-flip",
                        Name = "Back Flip",
                        Active = true,
                        Version = 1,
                        Description = "This is a test back flip",
                        Difficulty = "medium",
                        TrickCategories = new List<TrickCategory> {new TrickCategory {CategoryId = 2}},
                        Prerequisites = new List<TrickRelationship>
                        {
                            new TrickRelationship {PrerequisiteId = 1}
                        }
                    });
                    ctx.Add(new Submission
                    {
                        TrickId = "back-flip",
                        Description = "Test description, I've tried to go for max height",
                        Video = new Video
                        {
                            VideoLink = "https://localhost:5001/api/files/video/one.mp4",
                            ThumbLink = "https://localhost:5001/api/files/image/one.jpg"
                        },
                        VideoProcessed = true,
                        UserId = testUser.Id,
                    });
                    ctx.Add(new Submission
                    {
                        TrickId = "back-flip",
                        Description = "Test description, I've tried to go for min height",
                        Video = new Video
                        {
                            VideoLink = "https://localhost:5001/api/files/video/two.mp4",
                            ThumbLink = "https://localhost:5001/api/files/image/two.jpg"
                        },
                        VideoProcessed = true,
                        UserId = testUser.Id,
                    });
                    ctx.Add(new ModerationItem
                    {
                        Target = 3,
                        Type = ModerationTypes.Trick,
                    });
                    ctx.Add(new Video
                    {
                        VideoLink = "one.mp4",
                        ThumbLink = "one.jpg"
                    });
                    ctx.Add(new Video
                    {
                        VideoLink = "two.mp4",
                        ThumbLink = "two.jpg"
                    });
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