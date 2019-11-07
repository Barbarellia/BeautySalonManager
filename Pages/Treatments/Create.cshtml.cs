using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeautySalonManager.Models;

namespace BeautySalonManager.Pages.Treatments
{
    public class CreateModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public CreateModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Treatment Treatment { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Treatment.Add(Treatment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}