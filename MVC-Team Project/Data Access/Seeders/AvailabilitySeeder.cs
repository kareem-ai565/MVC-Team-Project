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
            var startTime = new TimeSpan(16, 0, 0); // 4:00 PM
            var endTime = new TimeSpan(20, 0, 0);   // 8:00 PM
            int slotDuration = 15; // 15 minutes
            int maxPatientsPerSlot = (int)((endTime - startTime).TotalMinutes / slotDuration); // 16 slots

            for (int doctorId = 1; doctorId <= 25; doctorId++)
            {
                for (int dayOffset = 0; dayOffset < 7; dayOffset++)
                {
                    availabilities.Add(new Availability
                    {
                        DoctorId = doctorId,
                        AvailableDate = DateTime.Today.AddDays(dayOffset),
                        StartTime = startTime,
                        EndTime = endTime,
                        SlotDuration = slotDuration,
                        MaxPatients = maxPatientsPerSlot,
                        IsBooked = false,
                        CreatedAt = DateTime.Now.AddDays(-dayOffset)
                    });
                }
            }

            context.Availabilities.AddRange(availabilities);
            context.SaveChanges();

            Console.WriteLine("Seeded availability for 25 doctors successfully.");
        }
    }
}
