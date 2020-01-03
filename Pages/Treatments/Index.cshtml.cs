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
        public PaginatedList<Enrollment> Enrollments { get; set; }
        public List<DateTime> FreePeriods { get; set; }
        public int TreatmentID { get; set; }
        public int EmployeeID { get; set; }

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
                Enrollments = PaginatedList<Enrollment>.Create(enrolIQ.AsNoTracking(), GetMonday(enrolIQ.FirstOrDefault().Date), pageIndex ?? 1, totalPages);
                Enrollments.FreePeriods = GetFreePeriods(Enrollments, TreatmentID);
            }
        }

        public DateTime GetMonday(DateTime date)
        {
            return date.Date.AddDays(-1 * (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7).Date;
        }

        public List<DateTime> GetFreePeriods(PaginatedList<Enrollment> enrollments, int treatmentId)
        {
            //list - paginated list of enrollments (list of start&end dates of each enrollment within 1 week)
            List<Tuple<DateTime, DateTime>> list = new List<Tuple<DateTime, DateTime>>();
            TimeSpan duration = _context.Treatment.FirstOrDefault(t => t.Id == treatmentId).Duration;

            //do kazdego enrolla dodaj czas jego trwania np. 10.00-11.30
            foreach (var enrol in enrollments)
            {
                list.Add(new Tuple<DateTime, DateTime>(enrol.Date, enrol.Date.Add(enrol.TreatmentAssignment.Treatment.Duration)));
            }

            List<DateTime> freeHours = new List<DateTime>();

            for (int i = 0; i < list.Count(); i++)
            {
                int year = list[i].Item1.Year;
                int month = list[i].Item1.Month;
                int day = list[i].Item1.Day;

                DateTime startDay = new DateTime(year, month, day, 9, 0, 0);
                DateTime endDay = new DateTime(year, month, day, 17, 0, 0);
                DateTime newTime;

                //jesli i=0 LUB zmiana dnia:
                if (i == 0 || (i != 0 && list[i - 1].Item1.Day != list[i].Item1.Day))
                {
                    if (list[i].Item1 - startDay >= duration)
                    {
                        freeHours.Add(list[i].Item1.Subtract(duration));
                        newTime = list[i].Item1.Subtract(duration);

                        while (newTime - startDay > duration)
                        {
                            freeHours.Add(newTime.Subtract(duration));
                            newTime = newTime.Subtract(duration);
                        }
                    }
                }
                //jesli ten sam dzien co poprzedni: (I ten sam co nastepny termin (czyli w srodku dnia)):
                else if (list[i - 1].Item1.Day == list[i].Item1.Day)// && list[i+1].Item1.Day == list[i].Item1.Day)
                {
                    if (list[i].Item1 - list[i - 1].Item2 >= duration)
                    {
                        freeHours.Add(list[i].Item1.Subtract(duration));
                        newTime = list[i].Item1.Subtract(duration);

                        while (newTime - list[i - 1].Item2 >= duration)
                        {
                            freeHours.Add(newTime.Subtract(duration));
                            newTime = newTime.Subtract(duration);
                        }
                    }
                }
                //jesli ostatni element z listy LUB ( nie ostatni I nastepny dzien jest inny):
                if (i == list.Count() - 1 || ((i != list.Count() - 1) && (list[i + 1].Item1.Day != list[i].Item1.Day)))
                {
                    //szukamy wolnego za terminem
                    if (endDay - list[i].Item2 >= duration)
                    {
                        freeHours.Add(endDay.Subtract(duration));
                        newTime = endDay.Subtract(duration);

                        while (newTime - list[i].Item2 >= duration)
                        {
                            freeHours.Add(newTime.Subtract(duration));
                            newTime = newTime.Subtract(duration);
                        }
                    }
                }
            }
            return freeHours;
        }
    }
}
