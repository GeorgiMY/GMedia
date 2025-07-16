using GMedia.Data;
using GMedia.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GMedia.Controllers
{
	public class PostsController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<User> _userManager;

		public PostsController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Posts
		public async Task<IActionResult> Index()
		{
			Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Post, User> applicationDbContext = _context.Posts.Include(p => p.Author);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Posts/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Post? post = await _context.Posts
				.Include(p => p.Author)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// GET: Posts/Create
		public IActionResult Create()
		{
			ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");

			return View();
		}

		// POST: Posts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Text,Visibility")] Post post)
		{
			User user = await _userManager.GetUserAsync(User);

			post.Id = Guid.NewGuid();
			post.Author = user;
			post.AuthorId = user.Id;
			_context.Add(post);

			await _context.SaveChangesAsync();

			return Redirect($"{Request.Scheme}://{Request.Host}");
		}

		// GET: Posts/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Post? post = await _context.Posts.FindAsync(id);

			if (post == null)
			{
				return NotFound();
			}

			ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);

			return View(post);
		}

		// POST: Posts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id,AuthorId,Text,Visibility,CreatedAt")] Post post)
		{
			if (id != post.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(post);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PostExists(post.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}

				return Redirect($"{Request.Scheme}://{Request.Host}");
			}

			ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);

			return View(post);
		}

		// GET: Posts/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Post? post = await _context.Posts
				.Include(p => p.Author)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// POST: Posts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			Post? post = await _context.Posts.FindAsync(id);

			if (post != null)
			{
				_context.Posts.Remove(post);
			}

			await _context.SaveChangesAsync();

			return Redirect($"{Request.Scheme}://{Request.Host}");
		}

		private bool PostExists(Guid id)
		{
			return _context.Posts.Any(e => e.Id == id);
		}
	}
}
