using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautySalonManager.Models;

namespace BeautySalonManager.Data
{
    public class DbInitializer
    {
        public static void Initialize(SalonContext context)
        {
            context.Database.EnsureCreated();

            if (context.Treatment.Any())
            {
                return;
            }

            var treatments = new Treatment[]
            {
                new Treatment{Name="Korekta paznokci żelowych", Duration= new TimeSpan(1,20,0), Price=80},
                new Treatment{Name="Korekta paznokci hybrydowych", Duration= new TimeSpan(1,0,0), Price=60},
                new Treatment{Name="Założenie paznokci żelowych", Duration= new TimeSpan(1,0,0), Price=80},
                new Treatment{Name="Korekta paznokci hybrydowych", Duration= new TimeSpan(1,0,0), Price=100}
            };
            foreach (Treatment t in treatments)
            {
                context.Treatment.Add(t);
            }
            context.SaveChanges();

            var employees = new Employee[]
            {
                new Employee{UserId=1},
                new Employee{UserId=2}
            };
            foreach(Employee e in employees)
            {
                context.Employee.Add(e);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment{UserId=1, TreatmentAssignmentId=1, Date=DateTime.Parse("2019-11-06 12:00:00"), Active=true},
                new Enrollment{UserId=1, TreatmentAssignmentId=2, Date=DateTime.Parse("2019-11-06 13:00:00"), Active=true}
            };
            foreach(Enrollment e in enrollments)
            {
                context.Enrollment.Add(e);
            }
            context.SaveChanges();
        }
    }
}
