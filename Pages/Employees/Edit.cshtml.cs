using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeautySalonManager.Pages.Employees
{
    [Authorize]
    public class EditModel : EmployeeTreatmentsPageModel
    {
        private readonly SalonContext _context;

        public EditModel(SalonContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee = await _context.Employee
                .Include(q => q.User)
                .Include(t => t.TreatmentAssignments).ThenInclude(q => q.Treatment)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }
            PopulateAssignedTreatmentData(_context, Employee);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedTreatments)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var employeeToUpdate = await _context.Employee
                .Include(q => q.User)
                .Include(t => t.TreatmentAssignments)
                    .ThenInclude(t => t.Treatment)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (await TryUpdateModelAsync<Employee>(
                employeeToUpdate,
                "Employee",
                i => i.TreatmentAssignments))
            {
                UpdateEmployeeTreatments(_context, selectedTreatments, employeeToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            UpdateEmployeeTreatments(_context, selectedTreatments, employeeToUpdate);
            PopulateAssignedTreatmentData(_context, employeeToUpdate);
            return Page();
        }
    }
}