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

            //po kliknieciu w dany treatment wyswietl przypisanych emloyees
            if (id != null)
            {
                TreatmentID = id.Value;
                Treatment treatment = Treatment.Treatments.Where(
                i => i.Id == id.Value).Single();
                Treatment.Employees = treatment.TreatmentAssignments.Select(s => s.Employee);
            }

            //po kliknieciu w danego employee wyswietl enrolle
            if (employeeId != null)
            {
                EmployeeID = employeeId.Value;
                Treatment.Enrollments = await _context.Enrollment
                    .Where(q => q.TreatmentAssignment.EmployeeId == employeeId && q.Active == true)
                    .Include(q => q.TreatmentAssignment)
                        .ThenInclude(q => q.Treatment)
                    .ToListAsync();                
            }

            //sa enrolle do wyswitlenia
            if (Treatment.Enrollments != null && Treatment.Enrollments.Count() != 0)
            {
                //znajdz poniedzialek w tym tygodniu
                DateTime thisWeekMonday = GetMonday(DateTime.Now);
                //ustaw enrolle wg daty
                IQueryable<Enrollment> enrolIQ = from e in Treatment.Enrollments
                                                 .AsQueryable()
                                                 .OrderBy(en => en.Date)
                                                 select e;

                //grupuj enrolle na tygodnie
                var enrolWeeksIQ = from e in enrolIQ
                                   group e by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(e.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) into weekGroup
                                   select weekGroup.Key;
                //tyle ile tygodni, tyle stron
                totalPages = enrolWeeksIQ.Count();
                if (enrolIQ.Any())
                {
                    //okreslamy ktory tydzien wyswietlic
                    currentWeek = pageIndex != null ? enrolWeeksIQ.ToList()[pageIndex.GetValueOrDefault() - 1]
                        : CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    //wybieramy enrolle z tego tygodnia
                    enrolIQ = enrolIQ.Where(e => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(e.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) == currentWeek);
                }
                Enrollments = PaginatedList<Enrollment>.Create(enrolIQ.AsNoTracking(), GetMonday(enrolIQ.FirstOrDefault().Date) ,pageIndex ?? 1, totalPages);
                var x = FreePeriods(Enrollments, TreatmentID);
            }
        }

        public DateTime GetMonday(DateTime date)
        {
            return date.Date.AddDays(-1 * (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7).Date;
        }

        public List<DateTime> FreePeriods(PaginatedList<Enrollment> enrollments, int treatmentId)
        {
            List<Tuple<DateTime, DateTime>> list = new List<Tuple<DateTime, DateTime>>();
            TimeSpan duration = _context.Treatment.FirstOrDefault(t => t.Id == treatmentId).Duration;

            foreach (var enrol in enrollments)
            { 
                list.Add(new Tuple<DateTime, DateTime>(enrol.Date, enrol.Date.Add(enrol.TreatmentAssignment.Treatment.Duration)));          
            }

            List<Tuple<DateTime, DateTime>> freePeriods = new List<Tuple<DateTime, DateTime>>();

            foreach(var item in list.GroupBy(l => l.Item1.Date))
            {
                var start = new DateTime(item.Key.Year, item.Key.Month, item.Key.Day, 9, 0, 0);
                var end = new DateTime(item.Key.Year, item.Key.Month, item.Key.Day, 17, 0, 0);
                freePeriods.Add(new Tuple<DateTime, DateTime>(start, end));
            }

            List<DateTime> freeHours = new List<DateTime>();

            for(int i=0; i<list.Count(); i++)
            { 
                int year = list[i].Item1.Year;
                int month = list[i].Item1.Month;
                int day = list[i].Item1.Day;

                if(i == 0)
                {
                    if(list[i].Item1 - duration <= new DateTime(year, month, day, 9, 0, 0))
                    {
                        freeHours.Add(new DateTime(year, month, day, 9, 0, 0));
                    }
                }
                else if(list[i].Item1 - duration > list[i - 1].Item2)
                {
                    freeHours.Add(list[i].Item1 - duration);
                }
            }
            return freeHours;
        }
    }
}
