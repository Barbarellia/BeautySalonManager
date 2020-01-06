using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BeautySalonManager.Pages.Treatments
{
    public class ConfirmEnrollmentModel : PageModel
    {
        private readonly SalonContext _context;

        public ConfirmEnrollmentModel(SalonContext context)
        {
            _context = context;
        }

        public DateTime EnrollmentDate { get; set; }
        public TreatmentAssignment TreatmentAssignment { get; set; }
        public int UserId { get; set; }
        public Enrollment Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(DateTime date, int treatmentAssignmentId, int userId)
        {
            var treatmentAssignment = await _context.TreatmentAssignment
                .FirstOrDefaultAsync(t => t.Id == treatmentAssignmentId);

            if(date == DateTime.MinValue 
                || treatmentAssignmentId == 0 
                || userId == 0 
                || treatmentAssignment == null)
            {
                return NotFound();
            }

            var newEnrollment = new Enrollment
            {

            };
            return Page();
        }
    }
}