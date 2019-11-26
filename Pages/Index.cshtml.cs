using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BeautySalonManager.Pages
{    
    public class IndexModel : PageModel
    {
        private readonly SalonContext _context;

        public IndexModel(SalonContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            //var emp = await _context.Employee.FirstOrDefaultAsync(q => q.Id == 1);
            //var x = FreePeriods(emp, new DateTime(2019, 11, 26, 9, 30, 0)).Result;
        }

        public async Task<List<Tuple<DateTime, DateTime>>> FreePeriods(Employee employee, DateTime date)
        {
            List<Tuple<DateTime, DateTime>> list = new List<Tuple<DateTime, DateTime>>();

            var empEnrolls = await _context.Enrollment
                .Where(q => q.TreatmentAssignment.Employee == employee
                && q.Date.Date == date.Date)
                .Include(q => q.TreatmentAssignment)
                    .ThenInclude(q => q.Treatment)
                .OrderBy(q=> q.Date)
                .ToListAsync();

            foreach(var enrol in empEnrolls)
            {
                list.Add(new Tuple<DateTime, DateTime>(enrol.Date, enrol.Date.Add(enrol.TreatmentAssignment.Treatment.Duration)));
            }
            return list;
        }
    }
}
