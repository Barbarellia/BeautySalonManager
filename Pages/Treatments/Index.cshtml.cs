using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;

namespace BeautySalonManager.Pages.Treatments
{
    public class IndexModel : PageModel
    {
        private readonly SalonContext _context;

        public IndexModel(SalonContext context)
        {
            _context = context;
        }

        public TreatmentIndexData Treatment { get; set; }
        public int TreatmentID { get; set; }
        public int EmployeeID { get; set; }

        public async Task OnGetAsync(int? id, int? employeeId)
        {
            Treatment = new TreatmentIndexData();
            Treatment.Treatments = await _context.Treatment
                  .Include(i => i.TreatmentAssignments)
                    .ThenInclude(i => i.Employee)
                        .ThenInclude(i => i.User)
                  .Include(i => i.TreatmentAssignments)
                    .ThenInclude(i => i.Enrollments)
                  .AsNoTracking()
                  .OrderBy(i => i.Name)
                  .ToListAsync();

            if (id != null)
            {
                TreatmentID = id.Value;
                Treatment treatment = Treatment.Treatments.Where(
                i => i.Id == id.Value).Single();
                Treatment.Employees = treatment.TreatmentAssignments.Select(s => s.Employee);
            }

            if (employeeId != null)
            {
                EmployeeID = employeeId.Value;
                Treatment.Enrollments = await _context.Enrollment
                    .Where(q => q.TreatmentAssignment.EmployeeId == employeeId && q.Active == true)
                    .Include(q => q.TreatmentAssignment)
                        .ThenInclude(q => q.Treatment)
                    .ToListAsync();                
            }

            if (Treatment.Enrollments != null && Treatment.Enrollments.Count() != 0)
            {
                var enrolledUserIds = Treatment.Enrollments.Select(q => q.UserId).ToList();
                Treatment.AppUsers = await _context.Users
                    .Where(i => enrolledUserIds.Contains(i.Id)).ToListAsync();
            }
        }
    }
}
