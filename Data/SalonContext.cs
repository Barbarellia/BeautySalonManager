using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeautySalonManager.Models
{
    public class SalonContext : IdentityDbContext
    {
        public SalonContext (DbContextOptions<SalonContext> options)
            : base(options)
        {
        }

        public DbSet<Treatment> Treatment { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
        public DbSet<TreatmentAssignment> TreatmentAssignment { get; set; }
    }
}
