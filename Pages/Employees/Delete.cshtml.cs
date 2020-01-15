using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeautySalonManager.Pages.Employees
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly SalonContext _context;

        public DeleteModel(SalonContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? employeeId)
        {
            if (employeeId == null)
            {
                return NotFound();
            }

            Employee = await _context.Employee
                .Include(q => q.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == employeeId);

            if (Employee == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? employeeId)
        {
            if (employeeId == null)
            {
                return NotFound();
            }

            Employee = await _context.Employee.FindAsync(employeeId);

            if (Employee != null)
            {
                _context.Employee.Remove(Employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { message = "Poprawnie usunięto pracownika" });
        }
    }
}
