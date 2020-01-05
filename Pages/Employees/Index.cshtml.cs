using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;
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

        public EmployeeIndexData Employee { get; set; }
        //public List<Enrollment> Enrollments { get; set; }
        public IList<Employee> Employees { get;set; }
        public int EmployeeID { get; set; }
        public int EnrollmentID { get; set; }

        public async Task OnGetAsync(int? id, int? employeeId)
        {
            Employee = new EmployeeIndexData();
            Employee.Employees = await _context.Employee
                .Include(e => e.User)
                .Include(e=>e.TreatmentAssignments)
                    .ThenInclude(e=>e.Enrollments)
                .AsNoTracking()
                .OrderBy(e=>e.Id)
                .ToListAsync();

            if(id != null)
            {
                EmployeeID = id.Value;
                Employee employee = Employee.Employees.Where(
                    e => e.Id == id.Value).Single();
                //Employee.Enrollments = employee.TreatmentAssignments.Select(
                  //  s => s.Enrollment);
            }
        }
    }
}
