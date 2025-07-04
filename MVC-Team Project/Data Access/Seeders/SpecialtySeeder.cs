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
                new Specialty { Name = "Cardiology", Description = "Heart related specialty", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Dermatology", Description = "Skin specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Orthopedics", Description = "Bone specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Neurology", Description = "Nervous system specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Pediatrics", Description = "Child specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Psychiatry", Description = "Mental health specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "General Surgery", Description = "General surgical procedures", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Oncology", Description = "Cancer specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Anesthesiology", Description = "Anesthesia specialist", IsActive = true, CreatedAt = DateTime.Now },
                new Specialty { Name = "Radiology", Description = "Imaging specialist", IsActive = true, CreatedAt = DateTime.Now }
            };

            context.Specialties.AddRange(specialties);
            context.SaveChanges();
        }
    }
}
