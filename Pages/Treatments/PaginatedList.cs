using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Pages.Treatments
{
    public class PaginatedList<T> : List<T>
    {
        [DisplayFormat(DataFormatString = "{0:dd.MM}", ApplyFormatInEditMode = false)]
        public DateTime Monday { get; private set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM}", ApplyFormatInEditMode = false)]
        public DateTime Sunday { get; private set; }
        //public DateTime EndDate { get; private set; }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, DateTime monday, int pageIndex, int totalPages)
        {
            Monday = monday;
            Sunday = monday.AddDays(6);
            TotalPages = totalPages;
            PageIndex = pageIndex;
            //EndDate = DateTime.Now.AddMonths(1).Date.AddDays(-1 * (7 + (DateTime.Now
            //    .DayOfWeek - DayOfWeek.Sunday)) % 7).Date;

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
            //get
            //{
            //    return (DateTime.Now.Date < Monday);
            //}
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
            //get
            //{
            //    return (Monday.AddDays(6).Date < EndDate);
            //}
        }

        public static PaginatedList<T> Create(
            IQueryable<T> source, DateTime monday, int pageIndex, int totalPages)
        {
            var items = source.ToList();
            return new PaginatedList<T>(items, monday, pageIndex, totalPages);
        }
    }
}
