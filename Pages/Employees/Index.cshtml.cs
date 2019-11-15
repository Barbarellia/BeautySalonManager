using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;

namespace BeautySalonManager.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public IndexModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; }

        public async Task OnGetAsync()
        {
            Employee = await _context.Employee.ToListAsync();
        }
    }
}
