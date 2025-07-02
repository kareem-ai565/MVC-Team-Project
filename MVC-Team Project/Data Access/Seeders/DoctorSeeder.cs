using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class DoctorSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Doctors.Any())
            {
                Console.WriteLine("Doctors table not empty, skipping seeding Doctors.");
                return;
            }

            var doctors = new List<Doctor>();

            for (int i = 1; i <= 10; i++)
            {
                doctors.Add(new Doctor
                {
                    UserId = i,
                    SpecialtyId = i,
                    Bio = $"Doctor bio {i}",
                    ClinicAddress = $"Clinic address {i}",
                    LicenseNumber = $"LIC-{1000 + i}",
                    ConsultationFee = 100 + i * 10,
                    ExperienceYears = i + 5,
                    Education = $"Education details {i}",
                    Certifications = $"Certifications {i}",
                    IsVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-i),
                });
            }

            context.Doctors.AddRange(doctors);
            context.SaveChanges();
        }
    }
}
