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

            int totalDoctors = 25;
            int totalSpecialties = 9;

            for (int i = 1; i <= totalDoctors; i++)
            {
                int specialtyId = ((i - 1) % totalSpecialties) + 1;

                doctors.Add(new Doctor
                {
                    UserId = i,
                    SpecialtyId = specialtyId,
                    Bio = $"Dr. {i} is a highly experienced doctor in specialty #{specialtyId}.",
                    ClinicAddress = $"Clinic #{i}, Street {i}, City",
                    LicenseNumber = $"LIC-{1000 + i}",
                    ConsultationFee = 150 + i * 5,
                    ExperienceYears = 3 + (i % 10),
                    Education = $"Graduated from Medical School #{(i % 5 + 1)}",
                    Certifications = $"Certified in Specialty #{specialtyId}",
                    IsVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-i)
                });
            }

            context.Doctors.AddRange(doctors);
            context.SaveChanges();

            Console.WriteLine("Seeded 25 doctors successfully.");
        }
    }
}
