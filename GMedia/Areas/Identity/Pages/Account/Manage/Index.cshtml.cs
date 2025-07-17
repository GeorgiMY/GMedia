// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using GMedia.Data;
using GMedia.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static GMedia.Data.Enums;

namespace GMedia.UI.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public Dictionary<string, string> AllAssociatedUsersAndStatus { get; set; }

        public IndexModel(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;

            AllAssociatedUsersAndStatus = new Dictionary<string, string>();
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            string userName = await _userManager.GetUserNameAsync(user);
            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            // Friends :)
            List<string> approvedFriendIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Accepted && (f.SenderId == user.Id || f.ResponderId == user.Id)).Select(f => f.SenderId == user.Id ? f.ResponderId : f.SenderId).ToListAsync();

            List<User> approvedFriends = await _context.Users.Where(u => approvedFriendIds.Contains(u.Id)).ToListAsync();

            // Pending sent invitations
            List<string> pendingSentIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Pending && f.SenderId == user.Id).Select(f => f.ResponderId).ToListAsync();

            List<User> pendingSent = await _context.Users.Where(u => pendingSentIds.Contains(u.Id)).ToListAsync();

            // Received invitations
            List<string> pendingReceivedIds = await _context.Friendships.Where(f => f.Status == FriendshipStatus.Pending && f.ResponderId == user.Id).Select(f => f.SenderId).ToListAsync();

            List<User> pendingReceived = await _context.Users.Where(u => pendingReceivedIds.Contains(u.Id)).ToListAsync();

            foreach (User u in pendingReceived) AllAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Pending Received";

            foreach (User u in pendingSent) AllAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Pending Sent";

            foreach (User u in approvedFriends) AllAssociatedUsersAndStatus[$"{u.UserName}|{u.Id}"] = "Friends";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            User user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
