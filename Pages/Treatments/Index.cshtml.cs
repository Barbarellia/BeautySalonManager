using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;
using System.Globalization;

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
        public PaginatedList<Enrollment> Enrollments { get; set; }

        public async Task OnGetAsync(int? id, int? employeeId, int? pageIndex)
        {
            int totalPages = 1;
            int currentWeek;
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
                DateTime thisWeekMonday = GetMonday(DateTime.Now);
                IQueryable<Enrollment> enrolIQ = from e in Treatment.Enrollments
                                                 .AsQueryable()
                                                 .OrderBy(en => en.Date)
                                                 select e;

                var enrolWeeksIQ = from e in enrolIQ
                                   group e by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(e.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) into weekGroup
                                   select weekGroup.Key;

                totalPages = enrolWeeksIQ.Count();
                if (enrolIQ.Any())
                {
                    currentWeek = pageIndex != null ? enrolWeeksIQ.ToList()[pageIndex.GetValueOrDefault() - 1]
                        : CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    enrolIQ = enrolIQ.Where(e => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(e.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) == currentWeek);
                }
                Enrollments = PaginatedList<Enrollment>.Create(enrolIQ.AsNoTracking(), pageIndex ?? 1, totalPages);
            }
        }

        public DateTime GetMonday(DateTime date)
        {
            return date.Date.AddDays(-1 * (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7).Date;
        }
    }
}
