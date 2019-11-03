using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models
{
    public class TreatmentAssignment
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public int EmployeeId { get; set; }

        public Treatment Treatment { get; set; }
        public Employee Employee { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
