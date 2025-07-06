using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class PatientSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Patients.Any())
            {
                Console.WriteLine("Patients table not empty, skipping seeding Patients.");
                return;
            }

            var patients = new List<Patient>();

            var genders = new[] { "Male", "Female", "Other" };
            var bloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

            for (int i = 26; i <= 50; i++)
            {
                patients.Add(new Patient
                {
                    UserId = i, 
                    Gender = genders[i % genders.Length],
                    DOB = DateTime.Today.AddYears(-20 - (i % 10)),
                    Address = $"Building {i}, Street {i}, City",
                    EmergencyContact = $"Relative {i}",
                    EmergencyPhone = $"0100{i:D7}",
                    BloodType = bloodTypes[i % bloodTypes.Length],
                    Allergies = i % 2 == 0 ? "None" : "Pollen",
                    MedicalHistory = $"Chronic conditions history #{i}",
                    CurrentMedications = i % 3 == 0 ? $"Med {i}" : "None",
                    InsuranceProvider = $"HealthCare Inc {i % 5}",
                    InsurancePolicyNumber = $"POL-{2000 + i}",
                    CreatedAt = DateTime.Now.AddDays(-i)
                });
            }

            context.Patients.AddRange(patients);
            context.SaveChanges();

            Console.WriteLine("Seeded 25 patients successfully.");
        }
    }
}
