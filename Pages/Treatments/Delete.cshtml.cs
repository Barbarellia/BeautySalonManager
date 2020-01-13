using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeautySalonManager.Pages.Treatments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public DeleteModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Treatment = await _context.Treatment.FindAsync(id);

            if (Treatment != null)
            {
                _context.Treatment.Remove(Treatment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
