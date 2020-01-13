using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BeautySalonManager.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly SalonContext _context;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            SalonContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Email { get; set; }
        public List<Enrollment> Enrollments { get; set; }

        [Phone]
        [BindProperty]
        [Display(Name = "Numer tel.")]
        public string PhoneNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(string message)
        {
            if (message != null)
            {
                ModelState.AddModelError(string.Empty, message);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var enrollments = await _context.Enrollment.Where(e => e.UserId == user.Id)
                .Include(q => q.TreatmentAssignment)
                    .ThenInclude(q => q.Treatment)
                .Include(q => q.TreatmentAssignment)
                    .ThenInclude(q => q.Employee)
                        .ThenInclude(q => q.User)
                .OrderBy(q => q.Date)
                .AsNoTracking()
                .ToListAsync();

            Email = email;
            PhoneNumber = phoneNumber;
            Enrollments = enrollments;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {                
                return RedirectToPage("./Index", new { message = "Nieprawidłowy numer telefonu." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }
    }
}
