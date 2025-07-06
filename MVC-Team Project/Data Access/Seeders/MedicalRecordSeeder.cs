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
            var random = new Random();

            for (int patientId = 1; patientId <= 25; patientId++)
            {
                int doctorId = random.Next(1, 26); 

                records.Add(new MedicalRecord
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    RecordType = "General Checkup",
                    Diagnosis = $"Diagnosis for patient {patientId}",
                    Treatment = $"Treatment for patient {patientId}",
                    Prescription = $"Prescription #{patientId}",
                    TestResults = $"Test results for patient {patientId}",
                    Notes = $"Follow-up in 2 weeks.",
                    RecordDate = DateTime.Today.AddDays(-patientId),
                    IsConfidential = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now.AddDays(-patientId)
                });
            }

            context.MedicalRecords.AddRange(records);
            context.SaveChanges();

            Console.WriteLine("Seeded 25 medical records successfully.");
        }
    }
}
