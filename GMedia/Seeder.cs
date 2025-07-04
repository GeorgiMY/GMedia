using GMedia.Data;
using GMedia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using static GMedia.Enums;

namespace GMedia
{
    public class Seeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Users.Any() && context.Invitations.Any() && context.Posts.Any() && context.Friendships.Any()) return;

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            User admin = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                Names = "Administrator",
                Gender = Gender.Other,
                BirthDate = new DateTime(1980, 1, 1),
                Visibility = VisibilityOptions.FriendsOnly
            };

            await userManager.CreateAsync(admin, "Admin123!");

            await context.SaveChangesAsync();

            if (env.IsDevelopment())
            {
                User user1 = new User
                {
                    UserName = "alice",
                    Email = "alice@example.com",
                    Names = "Alice Smith",
                    Gender = Gender.Female,
                    BirthDate = new DateTime(1990, 1, 1),
                    Visibility = VisibilityOptions.Public
                };

                User user2 = new User
                {
                    UserName = "bob",
                    Email = "bob@example.com",
                    Names = "Bob Johnson",
                    Gender = Gender.Male,
                    BirthDate = new DateTime(1985, 5, 20),
                    Visibility = VisibilityOptions.FriendsOnly
                };

                User user3 = new User
                {
                    UserName = "carol",
                    Email = "carol@example.com",
                    Names = "Carol Williams",
                    Gender = Gender.Female,
                    BirthDate = new DateTime(1993, 7, 15),
                    Visibility = VisibilityOptions.FriendsOfFriends
                };

                await userManager.CreateAsync(user1, "Password123!");
                await userManager.CreateAsync(user2, "Password123!");
                await userManager.CreateAsync(user3, "Password123!");

                await context.SaveChangesAsync();

                context.Friendships.AddRange(
                    new Friendship
                    {
                        SenderId = user1.Id,
                        ResponderId = user2.Id,
                        SentAt = DateTime.UtcNow.AddDays(-3),
                        RespondedAt = DateTime.UtcNow.AddDays(-2),
                        Status = FriendshipStatus.Accepted,
                    },
                    new Friendship
                    {
                        SenderId = user2.Id,
                        ResponderId = user3.Id,
                        SentAt = DateTime.UtcNow,
                        RespondedAt = DateTime.UtcNow.AddDays(-2),
                        Status = FriendshipStatus.Rejected
                    }
                );

                context.Invitations.AddRange(
                    new Invitation
                    {
                        Code = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow,
                        CreatorOfInvitationId = user1.Id,
                        Email = "example@email.com"
                    },
                    new Invitation
                    {
                        Code = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow.AddDays(0.5),
                        CreatorOfInvitationId = user2.Id,
                        RegisteredUserId = user3.Id,
                        Email = "carol@example.com"
                    },
                    new Invitation
                    {
                        Code = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        CreatorOfInvitationId = user3.Id,
                        Email = "example@email.com"
                    }
                );

                context.Posts.AddRange(
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = user1.Id,
                        Text = "Hello, this is Alice's first post!",
                        Visibility = VisibilityOptions.Public,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = user2.Id,
                        Text = "Bob here! Just joined GMedia.",
                        Visibility = VisibilityOptions.FriendsOnly,
                        CreatedAt = DateTime.UtcNow.AddDays(-4)
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = user3.Id,
                        Text = "Carol's first post! Excited to be here.",
                        Visibility = VisibilityOptions.FriendsOfFriends,
                        CreatedAt = DateTime.UtcNow.AddDays(-22)
                    }
                );

                await context.SaveChangesAsync();

                Console.WriteLine("Database has been seeded");
            }
        }
    }
}
