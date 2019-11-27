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

            if (!context.Treatment.Any())
            {
                var treatments = new Treatment[]
                {
                    new Treatment{Name="Korekta paznokci żelowych", Duration= new TimeSpan(1,20,0), Price=80},
                    new Treatment{Name="Korekta paznokci hybrydowych", Duration= new TimeSpan(1,0,0), Price=60},
                    new Treatment{Name="Założenie paznokci żelowych", Duration= new TimeSpan(1,0,0), Price=80},
                    new Treatment{Name="Założenie paznokci hybrydowych", Duration= new TimeSpan(1,0,0), Price=100}
                };
                foreach (Treatment t in treatments)
                {
                    context.Treatment.Add(t);
                }
                context.SaveChanges();
            }
            if (!context.Users.Any())
            {
                var users = new AppUser[]
                {
                    new AppUser{FirstName="Adam", LastName="Małysz", UserName="adam@małysz.pl",
                        NormalizedUserName="ADAM@MAŁYSZ.PL",Email="adam@małysz.pl", NormalizedEmail="ADAM@MAŁYSZ.PL",
                        PasswordHash="AQAAAAEAACcQAAAAEF2+bzkrQQJxx/o91I+qPiZo+hx5kd9BbVUsvEWk3S7s6yxnrUoyGh4GjZaxUd58VQ==",
                        SecurityStamp="FD4VTY7VO3NXLA6RDQZ7UUTMICJGQAL6",ConcurrencyStamp="f47689b9-d29c-44e4-af26-00152339e041", 
                        PhoneNumber="123456789",LockoutEnabled=true,AccessFailedCount=0},

                    new AppUser{FirstName="Robert", LastName="Kubica", UserName="robert@kubica.pl",
                        NormalizedUserName="ROBERT@KUBICA.PL",Email="robert@kubica.pl", NormalizedEmail="ROBERT@KUBICA.PL",
                        PasswordHash="AQAAAAEAACcQAAAAEF2+bzkrQQJxx/o91I+qPiZo+hx5kd9BbVUsvEWk3S7s6yxnrUoyGh4GjZaxUd58VQ==",
                        SecurityStamp="ET5UKGXLXLEDNX54H6ZI6XYIP3PVNOKB",ConcurrencyStamp="37d393db-0376-46d1-85fc-85cb799ec6cd",
                        PhoneNumber="123456789",LockoutEnabled=true,AccessFailedCount=0},

                    new AppUser{FirstName="Filip", LastName="Kowalczyk", UserName="filip@kowalczyk.pl",
                        NormalizedUserName="FILIP@KOWALCZYK.PL",Email="filip@kowalczyk.pl", NormalizedEmail="FILIP@KOWALCZYK.PL",
                        PasswordHash="AQAAAAEAACcQAAAAEF2+bzkrQQJxx/o91I+qPiZo+hx5kd9BbVUsvEWk3S7s6yxnrUoyGh4GjZaxUd58VQ==",
                        SecurityStamp="JDU5TXBYQPKHLIC32L2QW5WQPMUUE2JW",ConcurrencyStamp="48049f66-d6c3-4f3d-a6c8-3d94ccc0bd57",
                        PhoneNumber="123456789",LockoutEnabled=true,AccessFailedCount=0},

                    new AppUser{FirstName="Basia", LastName="Szwedex", UserName="basia@szwedex.pl",
                        NormalizedUserName="BASIA@SZWEDEX.PL",Email="basia@szwedex.pl", NormalizedEmail="BASIA@SZWEDEX.PL",
                        PasswordHash="AQAAAAEAACcQAAAAEF2+bzkrQQJxx/o91I+qPiZo+hx5kd9BbVUsvEWk3S7s6yxnrUoyGh4GjZaxUd58VQ==",
                        SecurityStamp="JNVZNOMSXX63POQWXOIBZGMUSFKDTQRV",ConcurrencyStamp="ac38b4a1-5880-41b5-ae9f-a59998608f2d",
                        PhoneNumber="123456789",LockoutEnabled=true,AccessFailedCount=0}
                };
                foreach (AppUser u in users)
                {
                    context.Users.Add(u);
                }

                var emps = new Employee[]
                {
                    new Employee{UserId=1},
                    new Employee{UserId=2},
                    new Employee{UserId=3},
                    new Employee{UserId=4}
                };
                foreach (Employee e in emps)
                {
                    context.Employee.Add(e);
                }
                var treatmentAssignments = new TreatmentAssignment[]
                {
                    new TreatmentAssignment{EmployeeId=1,TreatmentId=1},
                    new TreatmentAssignment{EmployeeId=1,TreatmentId=2},
                    new TreatmentAssignment{EmployeeId=2,TreatmentId=2},
                    new TreatmentAssignment{EmployeeId=2,TreatmentId=3},
                    new TreatmentAssignment{EmployeeId=3,TreatmentId=3},
                    new TreatmentAssignment{EmployeeId=3,TreatmentId=4},
                    new TreatmentAssignment{EmployeeId=4,TreatmentId=4},
                    new TreatmentAssignment{EmployeeId=4,TreatmentId=1}
                };
                foreach (TreatmentAssignment ta in treatmentAssignments)
                {
                    context.TreatmentAssignment.Add(ta);
                }
                context.SaveChanges();
            }
        }
    }
}
