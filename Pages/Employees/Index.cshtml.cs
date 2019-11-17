using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Identity;

namespace BeautySalonManager.Pages.Employees
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

        public IList<Employee> Employees { get;set; }

        public async Task OnGetAsync()
        {
            Employees = await _context.Employee.Include(e => e.User).ToListAsync();
        }
    }
}
