using GMedia.Data;
using GMedia.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static GMedia.Enums;

namespace GMedia.Controllers
{
	public class UsersController : Controller
	{
		private readonly UserManager<User> _userManager;

		private readonly ApplicationDbContext _context;

		public UsersController(UserManager<User> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			User? user = await _userManager.GetUserAsync(User);
			string? userName = await _userManager.GetUserNameAsync(user);
			Dictionary<string, string> allAssociatedUsersAndStatus = new Dictionary<string, string>();

			// Friends :)
			List<string> approvedFriendIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Accepted && (f.SenderId == user.Id || f.ResponderId == user.Id)).Select(f => f.SenderId == user.Id ? f.ResponderId : f.SenderId).ToListAsync();

			List<User> approvedFriends = await _context.Users.Where(u => approvedFriendIds.Contains(u.Id)).ToListAsync();

			// Pending sent invitations
			List<string> pendingSentIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Pending && f.SenderId == user.Id).Select(f => f.ResponderId).ToListAsync();

			List<User> pendingSent = await _context.Users.Where(u => pendingSentIds.Contains(u.Id)).ToListAsync();

			// Received invitations
			List<string> pendingReceivedIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Pending && f.ResponderId == user.Id).Select(f => f.SenderId).ToListAsync();

			List<User> pendingReceived = await _context.Users.Where(u => pendingReceivedIds.Contains(u.Id)).ToListAsync();

			foreach (User u in pendingReceived)
			{
				allAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Pending Received";
			}


			foreach (User u in pendingSent)
			{
				allAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Pending Sent";
		}

			foreach (User u in approvedFriends)
			{
				allAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Friends";
			}

			return View(allAssociatedUsersAndStatus);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User? viewedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (viewedUser == null)
			{
				return NotFound();
			}

			User? currentUser = await _userManager.GetUserAsync(User);

			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				(f.SenderId == currentUser.Id && f.ResponderId == viewedUser.Id) ||
				(f.SenderId == viewedUser.Id && f.ResponderId == currentUser.Id)
			);

			UserDetailsViewModel userDetails = new UserDetailsViewModel
			{
				ViewedUser = viewedUser,
				Friendship = friendship
			};

			return View(userDetails);
		}

		public class UserDetailsViewModel
		{
			public User? ViewedUser { get; set; }
			public Friendship? Friendship { get; set; }
		}

		[HttpPost]
		public async Task<IActionResult> SendFriendRequest(string id)
		{
			User? currentUser = await _userManager.GetUserAsync(User);
			User? targetUser = await _context.Users.FindAsync(id);

			if (currentUser == null || targetUser == null || currentUser.Id == targetUser.Id)
			{
				return NotFound();
			}

			// Check if friendship already exists
			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				(f.SenderId == currentUser.Id && f.ResponderId == targetUser.Id) ||
				(f.SenderId == targetUser.Id && f.ResponderId == currentUser.Id)
			);

			if (friendship == null)
			{
				friendship = new Friendship
				{
					SenderId = currentUser.Id,
					ResponderId = targetUser.Id,
					Status = FriendshipStatus.Pending
				};
				_context.Friendships.Add(friendship);
			}
			else
			{
				friendship.Status = FriendshipStatus.Pending;
				friendship.SenderId = currentUser.Id;
				friendship.ResponderId = targetUser.Id;
				_context.Friendships.Update(friendship);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Details), new { id = targetUser.Id });
		}

		[HttpPost]
		public async Task<IActionResult> AcceptFriendRequest(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User? currentUser = await _userManager.GetUserAsync(User);
			User? viewedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (viewedUser == null || currentUser == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				f.SenderId == id && f.ResponderId == currentUser.Id &&
				f.Status == FriendshipStatus.Pending);

			if (friendship == null)
			{
				return BadRequest("No pending friend request found.");
			}

			friendship.Status = FriendshipStatus.Accepted;

			_context.Friendships.Update(friendship);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", new { id = viewedUser.Id });
		}

		[HttpPost]
		public async Task<IActionResult> RejectFriendRequest(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User currentUser = await _userManager.GetUserAsync(User);
			User viewedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (viewedUser == null || currentUser == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				f.SenderId == currentUser.Id &&
				f.ResponderId == viewedUser.Id &&
				f.Status == FriendshipStatus.Pending);

			if (friendship == null)
			{
				return BadRequest("No pending friend request found.");
			}

			friendship.Status = FriendshipStatus.Rejected;

			_context.Friendships.Update(friendship);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", new { id = viewedUser.Id });
		}

		[HttpPost]
		public async Task<IActionResult> WithdrawFriendRequest(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User currentUser = await _userManager.GetUserAsync(User);
			User viewedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (viewedUser == null || currentUser == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				f.SenderId == viewedUser.Id &&
				f.ResponderId == currentUser.Id &&
				f.Status == FriendshipStatus.Pending);

			if (friendship == null)
			{
				return BadRequest("No pending friend request found.");
			}

			friendship.Status = FriendshipStatus.Withdrawn;

			_context.Friendships.Update(friendship);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", new { id = viewedUser.Id });
		}

		[HttpPost]
		public async Task<IActionResult> RemoveFriend(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User? currentUser = await _userManager.GetUserAsync(User);
			User? viewedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (viewedUser == null || currentUser == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
				f.SenderId == id && f.ResponderId == currentUser.Id &&
				f.Status == FriendshipStatus.Accepted);

			if (friendship == null)
			{
				return BadRequest("No friend found.");
			}

			_context.Friendships.Remove(friendship);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", new { id = viewedUser.Id });
		}

	}
}
