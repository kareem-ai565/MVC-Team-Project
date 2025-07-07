using Microsoft.AspNetCore.Identity;
using MVC_Team_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class ApplicationUserSeeder
    {
        public static void Seed(
            ClinicSystemContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            if (context.Users.Any())
            {
                Console.WriteLine("Users table not empty, skipping seeding users.");
                return;
            }

            Console.WriteLine("Seeding users...");

            var userData = new (string FullName, string Email, string Password)[]
            {
                ("Ali Hassan", "ali1@domain.com", "Pass@123"),
                ("Mona Tarek", "mona2@domain.com", "Pass@123"),
                ("Khaled Mostafa", "khaled3@domain.com", "Pass@123"),
                ("Aya Ahmed", "aya4@domain.com", "Pass@123"),
                ("Omar Youssef", "omar5@domain.com", "Pass@123"),
                ("Sara Adel", "sara6@domain.com", "Pass@123"),
                ("Nour Ashraf", "nour7@domain.com", "Pass@123"),
                ("Mahmoud Nabil", "mahmoud8@domain.com", "Pass@123"),
                ("Fatma Ali", "fatma9@domain.com", "Pass@123"),
                ("Hazem Amr", "hazem10@domain.com", "Pass@123"),
                ("Yara Mostafa", "yara11@domain.com", "Pass@123"),
                ("Tamer Salah", "tamer12@domain.com", "Pass@123"),
                ("Esraa Helmy", "esraa13@domain.com", "Pass@123"),
                ("Amr Hamdy", "amr14@domain.com", "Pass@123"),
                ("Marwan Kamal", "marwan15@domain.com", "Pass@123"),
                ("Nesma Taha", "nesma16@domain.com", "Pass@123"),
                ("Rania Fathy", "rania17@domain.com", "Pass@123"),
                ("Abdelrahman Hany", "abdel18@domain.com", "Pass@123"),
                ("Heba Gamal", "heba19@domain.com", "Pass@123"),
                ("Karim Sayed", "karim20@domain.com", "Pass@123"),
                ("Mariam Samir", "mariam21@domain.com", "Pass@123"),
                ("Sherif Nabil", "sherif22@domain.com", "Pass@123"),
                ("Laila Emad", "laila23@domain.com", "Pass@123"),
                ("Ziad Ehab", "ziad24@domain.com", "Pass@123"),
                ("Noha Saeed", "noha25@domain.com", "Pass@123"),
                ("Walid Hatem", "walid26@domain.com", "Pass@123"),
                ("Salma Reda", "salma27@domain.com", "Pass@123"),
                ("Tarek Younis", "tarek28@domain.com", "Pass@123"),
                ("Reem Osama", "reem29@domain.com", "Pass@123"),
                ("Bassel Mostafa", "bassel30@domain.com", "Pass@123"),
                ("Nada Ezz", "nada31@domain.com", "Pass@123"),
                ("Ahmed Sherif", "ahmed32@domain.com", "Pass@123"),
                ("Dina Ayman", "dina33@domain.com", "Pass@123"),
                ("Mohamed Fathy", "mohamed34@domain.com", "Pass@123"),
                ("Sahar Refaat", "sahar35@domain.com", "Pass@123"),
                ("Hassan Ali", "hassan36@domain.com", "Pass@123"),
                ("Aya Sherif", "aya37@domain.com", "Pass@123"),
                ("Ibrahim Sabry", "ibrahim38@domain.com", "Pass@123"),
                ("Engy Khaled", "engy39@domain.com", "Pass@123"),
                ("Yassin Hassan", "yassin40@domain.com", "Pass@123"),
                ("Malak Nasser", "malak41@domain.com", "Pass@123"),
                ("Osama Ehab", "osama42@domain.com", "Pass@123"),
                ("Jana Hossam", "jana43@domain.com", "Pass@123"),
                ("Basma Ali", "basma44@domain.com", "Pass@123"),
                ("Mostafa Sami", "mostafa45@domain.com", "Pass@123"),
                ("Inas Gamal", "inas46@domain.com", "Pass@123"),
                ("Ramy Adel", "ramy47@domain.com", "Pass@123"),
                ("Lina Ayman", "lina48@domain.com", "Pass@123"),
                ("Hany Ismail", "hany49@domain.com", "Pass@123"),
                ("Yomna Hassan", "yomna50@domain.com", "Pass@123"),
            };

            var roles = new[] { "Doctor", "Patient" };

            // Create roles if not exist
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole<int>(role)).Wait();
                    Console.WriteLine($"✅ Created role: {role}");
                }
            }

            for (int i = 0; i < userData.Length; i++)
            {
                var (fullName, email, password) = userData[i];

                var user = new ApplicationUser
                {
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    FullName = fullName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                var result = userManager.CreateAsync(user, password).Result;

                if (!result.Succeeded)
                {
                    Console.WriteLine($"❌ Failed to create user: {email}");
                    foreach (var error in result.Errors)
                        Console.WriteLine($"   - {error.Description}");
                    continue;
                }

                var roleName = i < 25 ? "Doctor" : "Patient";

                var roleResult = userManager.AddToRoleAsync(user, roleName).Result;

                if (roleResult.Succeeded)
                    Console.WriteLine($"✅ Created {email} and assigned to role: {roleName}");
                else
                    Console.WriteLine($"⚠️ Created {email} but failed to assign role: {roleName}");
            }

            context.SaveChanges();
        }
    }
}
