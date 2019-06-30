using ATM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ATM.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ATMContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ATMContext>>()))
            {
                // Look for any movies.
                if (context.CreditCards.Any())
                {
                    return;   // DB has been seeded
                }

                context.CreditCards.AddRange(ATMContext.GetSeedingCards());
                context.SaveChanges();
            }
        }
    }
}
