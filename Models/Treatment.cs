using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        [Display(Name="Nazwa zabiegu")]
        public string Name { get; set; }
        [Display(Name = "Czas trwania zabiegu")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "Cena zabiegu")]
        public double Price { get; set; }

        public ICollection<TreatmentAssignment> TreatmentAssignments { get; set; }
    }
}
