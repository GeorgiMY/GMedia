using GMedia.Data;
using GMedia.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GMedia.Controllers
{
	[Authorize]
	public class FriendshipsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public FriendshipsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Friendships
		public async Task<IActionResult> Index()
		{
			Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Friendship, User?> applicationDbContext = _context.Friendships.Include(f => f.Responder).Include(f => f.Sender).Include(f => f.WithdrawnBy);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Friendships/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships
				.Include(f => f.Responder)
				.Include(f => f.Sender)
				.Include(f => f.WithdrawnBy)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (friendship == null)
			{
				return NotFound();
			}

			return View(friendship);
		}

		// GET: Friendships/Create
		public IActionResult Create()
		{
			ViewData["ResponderId"] = new SelectList(_context.Users, "Id", "Id");
			ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id");
			ViewData["WithdrawnById"] = new SelectList(_context.Users, "Id", "Id");
			return View();
		}

		// POST: Friendships/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,SentAt,SenderId,RespondedAt,ResponderId,WithdrawnAt,WithdrawnById,Status")] Friendship friendship)
		{
			if (ModelState.IsValid)
			{
				_context.Add(friendship);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["ResponderId"] = new SelectList(_context.Users, "Id", "Id", friendship.ResponderId);
			ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", friendship.SenderId);
			ViewData["WithdrawnById"] = new SelectList(_context.Users, "Id", "Id", friendship.WithdrawnById);
			return View(friendship);
		}

		// GET: Friendships/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships.FindAsync(id);

			if (friendship == null)
			{
				return NotFound();
			}

			ViewData["ResponderId"] = new SelectList(_context.Users, "Id", "Id", friendship.ResponderId);
			ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", friendship.SenderId);
			ViewData["WithdrawnById"] = new SelectList(_context.Users, "Id", "Id", friendship.WithdrawnById);
			return View(friendship);
		}

		// POST: Friendships/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,SentAt,SenderId,RespondedAt,ResponderId,WithdrawnAt,WithdrawnById,Status")] Friendship friendship)
		{
			if (id != friendship.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(friendship);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!FriendshipExists(friendship.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}

			ViewData["ResponderId"] = new SelectList(_context.Users, "Id", "Id", friendship.ResponderId);
			ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", friendship.SenderId);
			ViewData["WithdrawnById"] = new SelectList(_context.Users, "Id", "Id", friendship.WithdrawnById);
			return View(friendship);
		}

		// GET: Friendships/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Friendship? friendship = await _context.Friendships
				.Include(f => f.Responder)
				.Include(f => f.Sender)
				.Include(f => f.WithdrawnBy)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (friendship == null)
			{
				return NotFound();
			}

			return View(friendship);
		}

		// POST: Friendships/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Friendship? friendship = await _context.Friendships.FindAsync(id);

			if (friendship != null)
			{
				_context.Friendships.Remove(friendship);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool FriendshipExists(int id)
		{
			return _context.Friendships.Any(e => e.Id == id);
		}
	}
}
