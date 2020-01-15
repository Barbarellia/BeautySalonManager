using BeautySalonManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeautySalonManager.Data
{
    public class EnrollmentStatusHandler
    {
        public static void ChangePastEnrollmentsStatus(SalonContext context)
        {
            if (context.Enrollment.Any())
            {
                var activeEnrols = context.Enrollment
                    .Where(q => q.Active == true && q.Date < DateTime.Now)
                    .AsNoTracking()
                    .ToList();

                if (activeEnrols.Count() > 0)
                {
                    foreach (var activeEnrol in activeEnrols)
                    {
                        context.Attach(activeEnrol).State = EntityState.Modified;
                        activeEnrol.Active = false;
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }

            }
        }
    }
}
