﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeautySalonManager.Pages.Employees
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly SalonContext _context;

        public DetailsModel(SalonContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee = await _context.Employee
                .Include(q => q.User)
                .Include(q=>q.TreatmentAssignments)
                    .ThenInclude(q=>q.Treatment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
