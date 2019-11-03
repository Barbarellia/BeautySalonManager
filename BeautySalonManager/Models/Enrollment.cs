using System;
using System.Collections.Generic;
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
        public DateTime Date { get; set; }
        [Display(Name = "Czy wykonano")]
        public bool Active { get; set; }

        public TreatmentAssignment TreatmentAssignment { get; set; }
        //public User User { get; set; }
    }
}
