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

            for (int i = 1; i <= 10; i++)
            {
                patients.Add(new Patient
                {
                    UserId = i,
                    Gender = genders[i % genders.Length],
                    DOB = DateTime.Today.AddYears(-20 - i),
                    Address = $"Address #{i} Main Street, City",
                    EmergencyContact = $"Contact Person {i}",
                    EmergencyPhone = $"010000000{i:D2}",
                    BloodType = bloodTypes[i % bloodTypes.Length],
                    Allergies = "None",
                    MedicalHistory = $"Medical history details {i}",
                    CurrentMedications = $"Medications {i}",
                    InsuranceProvider = $"Insurance Co {i}",
                    InsurancePolicyNumber = $"POLICY-{1000 + i}",
                    CreatedAt = DateTime.Now.AddDays(-i * 10)
                });
            }

            context.Patients.AddRange(patients);
            context.SaveChanges();
        }
    }
}
