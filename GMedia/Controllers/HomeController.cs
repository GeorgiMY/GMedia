using GMedia.Data;
using GMedia.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

using static GMedia.Data.Enums;

namespace GMedia.UI.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<User> _userManager;

		public HomeController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			User currentUser = await _userManager.GetUserAsync(User);

			if (currentUser == null)
			{
				return Challenge();
			}

			List<string> friendsIds = await _context.Friendships.Where(f => (f.SenderId == currentUser.Id || f.ResponderId == currentUser.Id) && f.Status == FriendshipStatus.Accepted).Select(f => f.SenderId == currentUser.Id ? f.ResponderId : f.SenderId).Distinct().ToListAsync();

			List<string> friendsOfFriendsIds = await _context.Friendships.Where(f => (friendsIds.Contains(f.SenderId) || friendsIds.Contains(f.ResponderId)) && f.Status == FriendshipStatus.Accepted).Select(f => friendsIds.Contains(f.SenderId) ? f.ResponderId : f.SenderId).Distinct().ToListAsync();

			List<Post> postsQuery = await _context.Posts
				.Include(p => p.Author)
				.Where(p =>
					p.AuthorId == currentUser.Id ||
					(p.Visibility == VisibilityOptions.Public && p.AuthorId != currentUser.Id) ||
					(p.Visibility == VisibilityOptions.FriendsOnly && friendsIds.Contains(p.AuthorId)) ||
					(p.Visibility == VisibilityOptions.FriendsOfFriends && friendsOfFriendsIds.Contains(p.AuthorId))
				)
				.OrderByDescending(p => p.CreatedAt)
				.ToListAsync();

			int pageSize = 25;

			List<Post> pagedPosts = postsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			ViewData["CurrentPage"] = page;

			return View(pagedPosts);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Friendships()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
