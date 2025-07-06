using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class AppointmentSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Appointments.Any())
            {
                Console.WriteLine("Appointments table not empty, skipping seeding Appointments.");
                return;
            }

            var appointments = new List<Appointment>();

            var availabilities = context.Availabilities
                .Where(a => !a.IsBooked)
                .OrderBy(a => a.DoctorId)
                .ThenBy(a => a.AvailableDate)
                .ToList();

            int patientCounter = 1;

            foreach (var availability in availabilities)
            {
                var totalSlots = (int)((availability.EndTime - availability.StartTime).TotalMinutes / availability.SlotDuration);
                var slotStartTime = availability.StartTime;

                for (int i = 0; i < totalSlots && patientCounter <= 25; i++)
                {
                    var slotEndTime = slotStartTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));

                    appointments.Add(new Appointment
                    {
                        PatientId = patientCounter,
                        DoctorId = availability.DoctorId,
                        AppointmentDate = availability.AvailableDate,
                        StartTime = slotStartTime,
                        EndTime = slotEndTime,
                        Status = "Scheduled",
                        PatientNotes = $"Patient notes {patientCounter}",
                        Symptoms = $"Symptoms {patientCounter}",
                        Diagnosis = null,
                        Prescription = null,
                        DoctorNotes = null,
                        IsFollowUpRequired = false,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    });

                    availability.IsBooked = true;

                    slotStartTime = slotEndTime;
                    patientCounter++;
                }

                if (patientCounter > 25) break;
            }

            context.Appointments.AddRange(appointments);
            context.SaveChanges();
            context.SaveChanges();

            Console.WriteLine("Seeded appointments for first 25 patients based on availability.");
        }
    }
}
