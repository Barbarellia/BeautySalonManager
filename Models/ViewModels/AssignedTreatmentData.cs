using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models.ViewModels
{
    public class AssignedTreatmentData
    {
        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
