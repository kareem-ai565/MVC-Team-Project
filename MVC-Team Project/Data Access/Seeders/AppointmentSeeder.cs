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

            for (int i = 1; i <= 10; i++)
            {
                appointments.Add(new Appointment
                {
                    PatientId = i,
                    DoctorId = i,
                    AppointmentDate = DateTime.Today.AddDays(i),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Status = "Scheduled",
                    PatientNotes = $"Patient notes {i}",
                    Symptoms = $"Symptoms {i}",
                    Diagnosis = $"Diagnosis {i}",
                    Prescription = $"Prescription {i}",
                    DoctorNotes = $"Doctor notes {i}",
                    IsFollowUpRequired = false,
                    CreatedAt = DateTime.Now.AddDays(-i),
                    IsDeleted = false,
                });
            }

            context.Appointments.AddRange(appointments);
            context.SaveChanges();
        }
    }
}
