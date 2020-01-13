using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BeautySalonManager.Areas.Identity.Pages.Account.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly SalonContext _context;

        public DeleteModel(SalonContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Enrollment Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? enrolId)
        {
            if (enrolId == null)
            {
                return NotFound();
            }

            Enrollment = await _context.Enrollment
                .Include(q => q.TreatmentAssignment)
                    .ThenInclude(q => q.Treatment)
                .Include(q => q.TreatmentAssignment)
                    .ThenInclude(q => q.Employee)
                        .ThenInclude(q => q.User)                
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == enrolId);

            if (Enrollment == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? enrolId)
        {
            if (enrolId == null)
            {
                return NotFound();
            }

            Enrollment = await _context.Enrollment.FindAsync(enrolId);

            if (Enrollment != null)
            {
                _context.Enrollment.Remove(Enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { message = "Poprawnie odwo³ano wizytê." });
        }
    }
}
