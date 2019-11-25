using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;

namespace BeautySalonManager.Pages.Treatments
{
    public class IndexModel : PageModel
    {
        private readonly SalonContext _context;

        public IndexModel(SalonContext context)
        {
            _context = context;
        }

        public TreatmentIndexData Treatment { get; set; }
        public int TreatmentID { get; set; }

        public async Task OnGetAsync(int? id)
        {
            Treatment = new TreatmentIndexData();
            Treatment.Treatments = await _context.Treatment
                  .Include(i => i.TreatmentAssignments)
                    .ThenInclude(i => i.Employee)
                        .ThenInclude(i => i.User)
                  .AsNoTracking()
                  .OrderBy(i => i.Name)
                  .ToListAsync();

            if (id != null)
            {
                TreatmentID = id.Value;
            }
        }
    }
}
