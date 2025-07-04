using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class PaymentSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Payments.Any())
            {
                Console.WriteLine("Payments table not empty, skipping seeding Payments.");
                return;
            }

            var payments = new List<Payment>();

            var methods = new[] { "Cash", "Credit Card", "Insurance", "Paypal" };
            var statuses = new[] { "Pending", "Completed", "Failed", "Refunded" };

            for (int i = 1; i <= 10; i++)
            {
                payments.Add(new Payment
                {
                    PatientId = i,
                    DoctorId = i,
                    AppointmentId = i,
                    Amount = 100 + i * 20,
                    PaymentMethod = methods[i % methods.Length],
                    PaymentStatus = statuses[i % statuses.Length],
                    TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    PaymentDate = DateTime.Now.AddDays(-i * 3),
                    DueDate = DateTime.Now.AddDays(i * 2),
                    PaidDate = DateTime.Now.AddDays(-i * 2),
                    RefundAmount = null,
                    RefundDate = null,
                    Notes = $"Payment note {i}",
                    CreatedAt = DateTime.Now.AddDays(-i * 3)
                });
            }

            context.Payments.AddRange(payments);
            context.SaveChanges();
        }
    }
}
