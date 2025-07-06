using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class SpecialtySeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Specialties.Any())
            {
                Console.WriteLine("Specialties table not empty, skipping seeding Specialties.");
                return;
            }

            var specialties = new List<Specialty>()
            {
                new Specialty
                {
                    Name = "Cardiology",
                    Description = "Diagnosis and treatment of heart and blood vessel disorders.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Dermatology",
                    Description = "Specialized in skin, hair, and nail conditions.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Orthopedics",
                    Description = "Focuses on the musculoskeletal system: bones, joints, and muscles.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Neurology",
                    Description = "Deals with disorders of the nervous system.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Pediatrics",
                    Description = "Medical care for infants, children, and adolescents.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Psychiatry",
                    Description = "Focus on mental, emotional, and behavioral disorders.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Oncology",
                    Description = "Diagnosis and treatment of cancer.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Anesthesiology",
                    Description = "Pain relief and anesthesia during surgeries.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Specialty
                {
                    Name = "Obstetrics and Gynecology",
                    Description = "Women's reproductive health, pregnancy, and childbirth.",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
            };

            context.Specialties.AddRange(specialties);
            context.SaveChanges();
        }
    }
}
