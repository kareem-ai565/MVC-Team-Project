using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class AvailabilitySeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Availabilities.Any())
            {
                Console.WriteLine("Availabilities table not empty, skipping seeding Availabilities.");
                return;
            }

            var availabilities = new List<Availability>();

            for (int i = 1; i <= 10; i++)
            {
                availabilities.Add(new Availability
                {
                    DoctorId = i,
                    AvailableDate = DateTime.Today.AddDays(i),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    IsBooked = false,
                    SlotDuration = 30,
                    MaxPatients = 10,
                    CreatedAt = DateTime.Now.AddDays(-i),
                });
            }

            context.Availabilities.AddRange(availabilities);
            context.SaveChanges();
        }
    }
}
