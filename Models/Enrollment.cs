using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TreatmentAssignmentId { get; set; }
        [Display(Name = "Data")]
        [Required(ErrorMessage = "Pole '{0}' jest wymagane")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }
        [Display(Name = "Czy wykonano")]
        public bool Active { get; set; }

        public TreatmentAssignment TreatmentAssignment { get; set; }
    }
}
