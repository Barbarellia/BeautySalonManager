using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeautySalonManager.Models;

namespace BeautySalonManager.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public CreateModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(!_context.Users.Any(q => q.Email == Employee.User.Email))
            {
                return Page();
            }            

            var emptyEmployee = new Employee();
            emptyEmployee.User = _context.Users.FirstOrDefault(q => q.Email == Employee.User.Email);

            if (await TryUpdateModelAsync<Employee>(
                 emptyEmployee,
                 "employee",   // Prefix for form value.
                 s => s.Id, s => s.UserId))
            {
                _context.Employee.Add(emptyEmployee);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}