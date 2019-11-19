using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using BeautySalonManager.Data;
using BeautySalonManager.Models;
using BeautySalonManager.Models.ViewModels;

namespace BeautySalonManager.Pages.Employees
{
    public class EmployeeTreatmentsPageModel : PageModel
    {
        public List<AssignedTreatmentData> AssignedTreatmentDataList;

        public void PopulateAssignedTreatmentData (SalonContext context, Employee employee)
        {
            var allTreatments = context.Treatment;
            var employeeTreatments = new HashSet<int>(
              employee.TreatmentAssignments.Select(c => c.TreatmentId));
            AssignedTreatmentDataList = new List<AssignedTreatmentData>();
            foreach(var treatment in allTreatments)
            {
                AssignedTreatmentDataList.Add(new AssignedTreatmentData
                {
                    TreatmentId = treatment.Id,
                    Name = treatment.Name,
                    Assigned = employeeTreatments.Contains(treatment.Id)
                });
            }
        }

        public void UpdateEmployeeTreatments (SalonContext context, string[] selectedTreatments, Employee employeeToUpdate)
        {
            if (selectedTreatments == null)
            {
                employeeToUpdate.TreatmentAssignments = new List<TreatmentAssignment>();
                return;
            }

            var selectedTreatmentsHS = new HashSet<string>(selectedTreatments);
            var employeeTreatments = new HashSet<int>
                (employeeToUpdate.TreatmentAssignments.Select(t => t.Treatment.Id));

            foreach(var treatment in context.Treatment)
            {
                if (selectedTreatmentsHS.Contains(treatment.Id.ToString()))
                {
                    if (!employeeTreatments.Contains(treatment.Id))
                    {
                        employeeToUpdate.TreatmentAssignments.Add(
                            new TreatmentAssignment
                            {
                                TreatmentId = treatment.Id,
                                EmployeeId = employeeToUpdate.Id
                            });
                    }
                }
                else
                {
                    if (employeeTreatments.Contains(treatment.Id))
                    {
                        TreatmentAssignment treatmentToRemove = 
                            employeeToUpdate.TreatmentAssignments
                            .SingleOrDefault(i => i.TreatmentId == treatment.Id);
                        context.Remove(treatmentToRemove);
                    }
                }
            }
        }
    }
}
