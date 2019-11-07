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
    public class IndexModel : PageModel
    {
        private readonly BeautySalonManager.Models.SalonContext _context;

        public IndexModel(BeautySalonManager.Models.SalonContext context)
        {
            _context = context;
        }

        public IList<Treatment> Treatment { get;set; }

        public async Task OnGetAsync()
        {
            Treatment = await _context.Treatment.ToListAsync();
        }
    }
}
