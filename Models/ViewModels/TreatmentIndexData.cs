﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models.ViewModels
{
    public class TreatmentIndexData
    {
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<Treatment> Treatments { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public IEnumerable<AppUser> AppUsers { get; set; }
    }
}
