using GMedia.Data;
using GMedia.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using static GMedia.Data.Enums;

namespace GMedia.Services
{
	public class Seeder
	{
		public static async Task SeedData(ApplicationDbContext context, bool isDevelopment, IServiceScope scope)
		{
            if (context.Users.Any() && context.Invitations.Any() && context.Posts.Any() && context.Friendships.Any())
			{
				return;
			}

			User admin = new User
			{
				UserName = "admin",
				Email = "admin@example.com",
				Names = "Administrator",
				Gender = Gender.Other,
				BirthDate = new DateTime(1980, 1, 1),
				Visibility = VisibilityOptions.FriendsOnly
			};

            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await userManager.CreateAsync(admin, "Admin123!");
            await context.SaveChangesAsync();

			if (!isDevelopment)
			{
				return;
			}

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

            Random random = new Random();
            string[] quotes = 
            {
				"The only limit to our realization of tomorrow is our doubts of today.",
				"Do what you can, with what you have, where you are.",
				"Success is not final, failure is not fatal: It is the courage to continue that counts.",
				"In the middle of every difficulty lies opportunity.",
				"Happiness is not something ready-made. It comes from your own actions.",
				"Believe you can and you're halfway there.",
				"Life is 10% what happens to us and 90% how we react to it.",
				"Dream big and dare to fail.",
				"It always seems impossible until it's done.",
				"You miss 100% of the shots you don't take.",
				"What lies behind us and what lies before us are tiny matters compared to what lies within us.",
				"The best way to predict the future is to create it.",
				"Don't watch the clock; do what it does. Keep going.",
				"Everything you’ve ever wanted is on the other side of fear.",
				"I am not a product of my circumstances. I am a product of my decisions.",
				"Keep your face always toward the sunshine—and shadows will fall behind you.",
				"Hardships often prepare ordinary people for an extraordinary destiny.",
				"Your time is limited, so don’t waste it living someone else’s life.",
				"The future belongs to those who believe in the beauty of their dreams.",
				"Sometimes the bravest and most important thing you can do is just show up.",
				"Small deeds done are better than great deeds planned.",
				"Doubt kills more dreams than failure ever will.",
				"The way to get started is to quit talking and begin doing.",
				"Strive not to be a success, but rather to be of value.",
				"It does not matter how slowly you go as long as you do not stop.",
				"Quality means doing it right when no one is looking.",
				"You don’t have to be great to start, but you have to start to be great.",
				"The secret of getting ahead is getting started.",
				"The only way to do great work is to love what you do.",
				"Success usually comes to those who are too busy to be looking for it."
			};

            string[] userIds = { user1.Id, user2.Id, user3.Id };

            for (int i = 0; i < 30; i++)
            {
                int user = random.Next(userIds.Length);
                string text = quotes[i % quotes.Length];
                DateTime createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 30));

                context.Posts.Add(new Post
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userIds[user],
                    Text = text,
                    Visibility = VisibilityOptions.Public,
                    CreatedAt = createdAt
                });
            }

            await context.SaveChangesAsync();
		}
	}
}
