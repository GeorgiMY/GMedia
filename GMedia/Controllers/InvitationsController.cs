using GMedia.Data;
using GMedia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GMedia.Controllers
{
    [Authorize]
    public class InvitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public InvitationsController(ApplicationDbContext context, UserManager<User> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Invitations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invitations.Include(i => i.CreatorOfInvitation).Include(i => i.RegisteredUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invitations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations
                .Include(i => i.CreatorOfInvitation)
                .Include(i => i.RegisteredUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // GET: Invitations/Create
        public IActionResult Create()
        {
            //ViewData["CreatorOfInvitationId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["RegisteredUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email")] Invitation invitation)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            invitation.CreatorOfInvitation = currentUser;
            invitation.CreatorOfInvitationId = currentUser.Id;

            // Send invitation email
            await _emailSender.SendEmailAsync(invitation.Email, "You have been invited to GMedia",
                        $"You can create an account by <a href='{Request.Scheme}://{Request.Host.ToString()}/Identity/Account/Register?code={invitation.Code}'>clicking here</a>.");

            _context.Add(invitation);
            await _context.SaveChangesAsync();

            return Redirect($"{Request.Scheme}://{Request.Host.ToString()}");
        }

        // GET: Invitations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }
            ViewData["CreatorOfInvitationId"] = new SelectList(_context.Users, "Id", "Id", invitation.CreatorOfInvitationId);
            ViewData["RegisteredUserId"] = new SelectList(_context.Users, "Id", "Id", invitation.RegisteredUserId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,CreatorOfInvitationId,ExpirationDate,RegisteredUserId,Code")] Invitation invitation)
        {
            if (id != invitation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invitation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvitationExists(invitation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Redirect($"{Request.Scheme}://{Request.Host.ToString()}");
            }
            ViewData["CreatorOfInvitationId"] = new SelectList(_context.Users, "Id", "Id", invitation.CreatorOfInvitationId);
            ViewData["RegisteredUserId"] = new SelectList(_context.Users, "Id", "Id", invitation.RegisteredUserId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations
                .Include(i => i.CreatorOfInvitation)
                .Include(i => i.RegisteredUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation != null)
            {
                _context.Invitations.Remove(invitation);
            }

            await _context.SaveChangesAsync();
            return Redirect($"{Request.Scheme}://{Request.Host.ToString()}");
        }

        private bool InvitationExists(int id)
        {
            return _context.Invitations.Any(e => e.Id == id);
        }
    }
}
