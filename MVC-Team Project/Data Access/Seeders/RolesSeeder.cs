using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace MVC_Team_Project.Seeders
{
    public static class RoleSeeder
    {
        public static void Seed(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Patient" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    var result = roleManager.CreateAsync(new IdentityRole<int>(role)).Result;
                    Console.WriteLine(result.Succeeded
                        ? $"Role '{role}' created."
                        : $"Failed to create role '{role}'.");
                }
            }
        }
    }
}
