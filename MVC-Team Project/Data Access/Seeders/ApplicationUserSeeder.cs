using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class ApplicationUserSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            if (context.Users.Any()) {
                Console.WriteLine("Users table not empty, skipping seeding users.");
                return; 
            }
            Console.WriteLine("Seeding users...");

            var users = new List<ApplicationUser>();

            for (int i = 1; i <= 10; i++)
            {
                users.Add(new ApplicationUser
                {
                    UserName = $"user{i}@domain.com",
                    NormalizedUserName = $"USER{i}@DOMAIN.COM",
                    Email = $"user{i}@domain.com",
                    NormalizedEmail = $"USER{i}@DOMAIN.COM",
                    EmailConfirmed = true,
                    FullName = $"User FullName {i}",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = "123456789",
                    CreatedAt = DateTime.Now.AddDays(-i),
                    IsActive = true,
                    EmailVerified = true,
                    ProfilePicture = null,
                    LastLogin = DateTime.Now.AddDays(-i / 2),
                });
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
