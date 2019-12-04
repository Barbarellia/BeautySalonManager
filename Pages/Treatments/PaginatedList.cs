using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Pages.Treatments
{
    public class PaginatedList<T> : List<T>
    {
        public DateTime Monday { get; private set; }
        public DateTime Sunday { get; private set; }
        public DateTime EndDate { get; private set; }

        public PaginatedList(List<T> items, DateTime monday)
        {
            Monday = monday;
            Sunday = monday.AddDays(6);
            EndDate = DateTime.Now.AddMonths(1).Date.AddDays(-1 * (7 + (DateTime.Now
                .DayOfWeek - DayOfWeek.Sunday)) % 7).Date;

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (DateTime.Now.Date < Monday);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (Monday.AddDays(6).Date < EndDate);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, DateTime monday)
        {
            var items = await source.ToListAsync();
            return new PaginatedList<T>(items, monday);
        }
    }
}
