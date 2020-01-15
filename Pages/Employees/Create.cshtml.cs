using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeautySalonManager.Pages.Employees
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly SalonContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CreateModel(SalonContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid || !_context.Users.Any(
                q => q.Email == Employee.User.Email &&
                q.FirstName == Employee.User.FirstName &&
                q.LastName == Employee.User.LastName))
            {
                ModelState.AddModelError(string.Empty, "Nie znaleziono użytkownika.");
                return Page();
            }

            var user = _context.Users.FirstOrDefault(p => p.Email == Employee.User.Email);

            if (_context.Employee.Any(q => q.UserId == user.Id))
            {
                ModelState.AddModelError(string.Empty, "Pracownik już istnieje.");
                return Page();
            }

            var emptyEmployee = new Employee();
            emptyEmployee.User = _context.Users.FirstOrDefault(
                q => q.Email == Employee.User.Email &&
                q.FirstName == Employee.User.FirstName &&
                q.LastName == Employee.User.LastName);

            if (await TryUpdateModelAsync<Employee>(
                 emptyEmployee,
                 "employee",   // Prefix for form value.
                 s => s.Id, s => s.UserId))
            {
                _context.Employee.Add(emptyEmployee);
                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(emptyEmployee.User, "Employee");

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}