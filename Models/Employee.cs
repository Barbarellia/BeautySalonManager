using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        //public User User { get; set; }
        public ICollection<TreatmentAssignment> TreatmentAssignments { get; set; }
    }
}
