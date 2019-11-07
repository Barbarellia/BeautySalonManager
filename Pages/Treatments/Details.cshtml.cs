using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;

namespace BeautySalonManager.Pages.Treatments
{
    public class DetailsModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public DetailsModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        public Treatment Treatment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Treatment = await _context.Treatment.FirstOrDefaultAsync(m => m.Id == id);

            if (Treatment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
