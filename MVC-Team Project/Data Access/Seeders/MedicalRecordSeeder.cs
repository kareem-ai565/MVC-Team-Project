using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class MedicalRecordSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.MedicalRecords.Any())
            {
                Console.WriteLine("MedicalRecords table not empty, skipping seeding MedicalRecords.");
                return;
            }

            var records = new List<MedicalRecord>();

            for (int i = 1; i <= 10; i++)
            {
                records.Add(new MedicalRecord
                {
                    PatientId = i,
                    DoctorId = i,
                    RecordType = "General Checkup",
                    Diagnosis = $"Diagnosis detail {i}",
                    Treatment = $"Treatment detail {i}",
                    Prescription = $"Prescription detail {i}",
                    TestResults = $"Test results {i}",
                    Notes = $"Notes {i}",
                    RecordDate = DateTime.Today.AddDays(-i * 10),
                    IsConfidential = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now.AddDays(-i * 10),
                });
            }

            context.MedicalRecords.AddRange(records);
            context.SaveChanges();
        }
    }
}
