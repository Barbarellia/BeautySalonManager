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
        [Required]
        [Display(Name="Nazwa zabiegu")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Czas trwania zabiegu")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Duration { get; set; }
        [Required]
        [Display(Name = "Cena zabiegu")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Kategoria zabiegu")]
        public int Category { get; set; }

        public ICollection<TreatmentAssignment> TreatmentAssignments { get; set; }
    }
}
