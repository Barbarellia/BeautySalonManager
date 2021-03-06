﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BeautySalonManager.Pages.Treatments
{
    public class IndexModel : PageModel
    {
        private readonly SalonContext _context;
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(SalonContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public TreatmentIndexData Treatment { get; set; }
        //public PaginatedList<Enrollment> Enrollments { get; set; }
        public List<DateTime> FreePeriods { get; set; }
        public int TreatmentID { get; set; }
        public int EmployeeID { get; set; }
        
        public Tuple<int, string, int>[] MonthsNavigation { get; set; }
        public string SelectedMonth { get; set; }
        public int SelectedMonthInt { get; set; }
        public int SelectedDay { get; set; }
        public int SelectedMonthDaysNumber { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public int TreatmentAssignmentId { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? treatmentId, int? employeeId, string month, int? day, string message)
        {
            if(message != null)
            {
                ModelState.AddModelError(string.Empty, message);
            }
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

            // definiuje tupla przechowującego informacje o (roku, nazwie miesiąca, liczbie dni w tym miesiącu, w tym roku).
            // służy do wyswietlania 3 nastepnych miesiecy na butonach i butonów odpowiadająchych dniom
            Tuple<int, string, int>[] monthsDays = new Tuple<int, string, int>[3];
            int monthButtonYear;
            string monthButtonMonthName;
            int monthButtonDaysInMonth;

            for (int i = 0; i < monthsDays.Length; i++) //monthDays.Length = 3
            {
                monthButtonYear = DateTime.Now.AddMonths(i).Year;
                monthButtonMonthName = DateTime.Now.AddMonths(i).ToString("MMMM", CultureInfo.CurrentCulture);  //zamienia miesiąc na polską nazwe
                monthButtonMonthName = char.ToUpper(monthButtonMonthName[0]) + monthButtonMonthName.Substring(1);   //zamienia pierwszą literę na wielką

                monthButtonDaysInMonth = DateTime.DaysInMonth(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month);
                Tuple<int, string, int> monthAndDays = new Tuple<int, string, int>(monthButtonYear, monthButtonMonthName, monthButtonDaysInMonth);
                monthsDays[i] = monthAndDays;
            }

            MonthsNavigation = monthsDays;

            //po kliknieciu w dany treatment wyjmij przypisanych employees
            if (treatmentId != null)
            {
                TreatmentID = treatmentId.Value;
                Treatment treatment = Treatment.Treatments.Where(i => i.Id == treatmentId.Value).Single();
                Treatment.Employees = treatment.TreatmentAssignments.Select(s => s.Employee);
            }

            //po kliknieciu w danego employee wyjmij enrolle
            if (employeeId != null)
            {
                EmployeeID = employeeId.Value;                    
            }

            if (!String.IsNullOrWhiteSpace(month))   //jesli został wcisniety buton z miesiącem
            {
                SelectedMonth = month;
                SelectedMonthDaysNumber = MonthsNavigation.FirstOrDefault(m => m.Item2 == month).Item3; //bierzemy ilosc dni tego miesiaca, ktory kliknelismy
                SelectedMonthInt = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            }

            if (day != null)    //jesli zostal wybrany konkretny dzien
            {
                SelectedDay = day.Value;
                //pobranie wszystkich danych aby zdefiniowac interesujący nas DateTime (year,month,day)
                int year = MonthsNavigation.FirstOrDefault(m => m.Item2 == month).Item1;           

                DateTime selectedDate = new DateTime(year,SelectedMonthInt,day.Value);

                //var enrollments = Treatment.Enrollments.Where(e => e.Date.Date == selectedDate.Date).ToList();  //wybieranie tych enrolów ktore są w wybranym dniu
                var enrollments = await _context.Enrollment
                                            .Where(q => q.TreatmentAssignment.EmployeeId == employeeId
                                                && q.Active == true
                                                && q.Date.Date == selectedDate.Date)
                                            .Include(q => q.TreatmentAssignment)
                                                .ThenInclude(q => q.Treatment)
                                            .ToListAsync();
                Enrollments = enrollments;

                FreePeriods = GetFreePeriods2(enrollments, selectedDate, treatmentId.Value);
                FreePeriods.Sort();

                var ta = _context.TreatmentAssignment
                    .FirstOrDefault(q => q.EmployeeId == employeeId && q.TreatmentId == treatmentId);
                if (ta != null)
                    TreatmentAssignmentId = ta.Id;

                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return LocalRedirect("/Identity/Account/Login");
                }
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                var userId = user.Id;
                UserId = userId;
            }
            return null;           
        }

        //-----------------------------TODO-------------------------
        public List<DateTime> GetFreePeriods2(List<Enrollment> enrollments, DateTime selectedDate, int treatmentId) 
        {
            List<Tuple<DateTime, DateTime>> workingPeriods = new List<Tuple<DateTime, DateTime>>();
            TimeSpan duration = _context.Treatment.FirstOrDefault(t => t.Id == treatmentId).Duration;
            List<DateTime> freeHours = new List<DateTime>();

            DateTime startTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 9, 0, 0);
            DateTime endTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 17, 0, 0);

            if (enrollments.Count() > 0)
            {
                foreach (var enrol in enrollments)
                {
                    //kazdy enroll dopisz do listy
                    workingPeriods.Add(new Tuple<DateTime, DateTime>(enrol.Date, enrol.Date.Add(enrol.TreatmentAssignment.Treatment.Duration)));
                }
                workingPeriods.Sort();
                for (int i = 0; i < workingPeriods.Count(); i++)
                {
                    if (i == 0)
                    {
                        SearchForHours(workingPeriods[i].Item1, startTime, duration, freeHours);
                    }
                    else
                    {
                        SearchForHours(workingPeriods[i].Item1, workingPeriods[i - 1].Item2, duration, freeHours);
                    }
                    if(i == workingPeriods.Count() - 1)
                    {
                        SearchForHours(endTime, workingPeriods[i].Item2, duration, freeHours);
                    }
                }

            }
            else
            {
                SearchForHours(endTime, startTime, duration, freeHours);
            }

            return freeHours;
        }
        private void SearchForHours(DateTime subtractItemLeft, DateTime subtractItemRight, TimeSpan duration, List<DateTime> freeHours)
        {
            DateTime newTime = new DateTime();

            if (subtractItemLeft - subtractItemRight >= duration)
            {
                freeHours.Add(subtractItemLeft.Subtract(duration));
                newTime = subtractItemLeft.Subtract(duration);

                while (newTime - subtractItemRight >= duration)
                {
                    freeHours.Add(newTime.Subtract(duration));
                    newTime = newTime.Subtract(duration);
                }
            }
        }

        public DateTime GetMonday(DateTime date)
        {
            DateTime monday = date.Date.AddDays(-1 * (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7).Date;
            return monday;
        }

        public int GetSunday(DateTime date)
        {
            DateTime monday = GetMonday(date);
            int daysInMonth = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(monday.Year, monday.Month);
            if (monday.Day + 6 < daysInMonth)
            {
                return monday.Day + 6;
            }
            else
            {
                return (monday.Day + 6) % daysInMonth;
            }
        }

        public List<DateTime> GetFreePeriods(PaginatedList<Enrollment> enrollments, int treatmentId, int currentWeek)
        {
            //list - paginated list of enrollments (list of start&end dates of each enrollment within 1 week)
            List<Tuple<DateTime, DateTime>> list = new List<Tuple<DateTime, DateTime>>();
            TimeSpan duration = _context.Treatment.FirstOrDefault(t => t.Id == treatmentId).Duration;
            List<DateTime> freeHours = new List<DateTime>();
            List<int> enrollmentDays = new List<int>();

            //kazdy enroll dopisz do listy
            foreach (var enrol in enrollments)
            {
                list.Add(new Tuple<DateTime, DateTime>(enrol.Date, enrol.Date.Add(enrol.TreatmentAssignment.Treatment.Duration)));
            }

            //zapisz wszystkie nry dni, ktore maja zapisane enrollmenty
            for (int i= 0; i < list.Count(); i++)
            {
                if(!enrollmentDays.Contains(list[i].Item1.Day))
                {
                    enrollmentDays.Add(list[i].Item1.Day);
                }
            }

            //jesli podany tydzien to biezacy tydzien roku
            if (CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay,
                            DayOfWeek.Monday) == currentWeek)
            {
                //wez dzisiejszy dzien jako poczatek i wyszukaj niedziele
                int beginningOfWeek = DateTime.Now.Day;
                int endOfWeek = GetSunday(DateTime.Now);
                FillEmptyDays(beginningOfWeek, endOfWeek, duration, enrollmentDays, list, freeHours);
            }
            else
            {
                //wez dowolny element z listy i wyszukaj pon i nd
                int beginningOfWeek = GetMonday(list[0].Item1).Day;
                int endOfWeek = GetSunday(list[0].Item1);
                FillEmptyDays(beginningOfWeek, endOfWeek, duration, enrollmentDays, list, freeHours);
            }
            
            for (int i = 0; i < list.Count(); i++)
            {
                int year = list[i].Item1.Year;
                int month = list[i].Item1.Month;
                int day = list[i].Item1.Day;

                DateTime startDay = new DateTime(year, month, day, 9, 0, 0);
                DateTime endDay = new DateTime(year, month, day, 17, 0, 0);

                //jesli i=0 LUB zmiana dnia:
                if (i == 0 || (i != 0 && list[i - 1].Item1.Day != list[i].Item1.Day))
                {
                    SearchForHours(list[i].Item1, startDay, duration, freeHours);
                }
                //jesli ten sam dzien co poprzedni:
                else if (list[i - 1].Item1.Day == list[i].Item1.Day)
                {
                    SearchForHours(list[i].Item1, list[i - 1].Item2, duration, freeHours);
                }
                //jesli ostatni element z listy LUB ( nie ostatni I nastepny dzien jest inny):
                if (i == list.Count() - 1 || ((i != list.Count() - 1) && (list[i + 1].Item1.Day != list[i].Item1.Day)))
                {
                    SearchForHours(endDay, list[i].Item2, duration, freeHours);
                }

            }
            return freeHours;
        }


        private void FillEmptyDays(int beginningOfWeek, int endOfWeek, TimeSpan duration, List<int> enrollmentDays, List<Tuple<DateTime,DateTime>> list, List<DateTime> freeHours)
        {
            for (int i = beginningOfWeek; i <= endOfWeek; i++)
            {
                //sprawdz czy nr dnia od poczatkowego dnia do piatku sa na liscie
                //jesli ktoregos nru dnia nie ma, wypelnij caly dzien wolnymi terminami
                if (!enrollmentDays.Contains(i))
                {
                    //rok i miesiac zapisz taki, jaki jest w pierwszym elemencie listy
                    int year = list[0].Item1.Year;
                    int month = list[0].Item1.Month;
                    //jesli dany dzien jest mniejszy od poprzedniego (przelom miesiaca np i-1=31, i=1)
                    //dodaj miesiac+1
                    if (i < i - 1) { month++; }
                    //jesli wyszlo ze miesiac = 13, to ustaw styczen nowego roku
                    if (month == 13)
                    {
                        month = 1;
                        year++;
                    }
                    DateTime startDay = new DateTime(year, month, i, 9, 0, 0);
                    DateTime endDay = new DateTime(year, month, i, 17, 0, 0);
                    //wypelnij caly dzien wolnymi terminami
                    SearchForHours(startDay, endDay, duration, freeHours);
                }
            }
        }
    }
}
