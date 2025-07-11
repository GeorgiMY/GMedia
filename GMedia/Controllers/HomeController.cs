using GMedia.Data;
using GMedia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static GMedia.Enums;

namespace GMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            User currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null) return Challenge();

            List<string> friendsIds = await _context.Friendships.Where(f => (f.SenderId == currentUser.Id || f.ResponderId == currentUser.Id) && f.Status == FriendshipStatus.Accepted).Select(f => f.SenderId == currentUser.Id ? f.ResponderId : f.SenderId).Distinct().ToListAsync();

            List<string> friendsOfFriendsIds = await _context.Friendships.Where(f => (friendsIds.Contains(f.SenderId) || friendsIds.Contains(f.ResponderId)) && f.Status == FriendshipStatus.Accepted).Select(f => friendsIds.Contains(f.SenderId) ? f.ResponderId : f.SenderId).Distinct().ToListAsync();

            List<string> usersToInclude = friendsIds.Concat(friendsOfFriendsIds).Append(currentUser.Id).Distinct().ToList();

            List<Post> postsQuery = await _context.Posts.Include(p => p.Author).Where(p => usersToInclude.Contains(p.AuthorId))
                .Where(p =>
                    p.Visibility == VisibilityOptions.Public ||
                    (p.Visibility == VisibilityOptions.FriendsOnly && friendsIds.Contains(p.AuthorId)) ||
                    p.AuthorId == currentUser.Id
                )
                .OrderByDescending(p => p.CreatedAt).ToListAsync();

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
