using System;
using Microsoft.AspNetCore.Identity;
using MVC_Team_Project.Models;

namespace MVC_Team_Project.Seeders
{
    public static class DbSeeder
    {
        public static void Seed(ClinicSystemContext context, UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<int>> roleManager)
        {
            RoleSeeder.Seed(roleManager);
            ApplicationUserSeeder.Seed(context, userManager, roleManager);
            SpecialtySeeder.Seed(context);
            DoctorSeeder.Seed(context);
            AvailabilitySeeder.Seed(context);
            PatientSeeder.Seed(context);
            AppointmentSeeder.Seed(context);
            PaymentSeeder.Seed(context);
            MedicalRecordSeeder.Seed(context);
        }
    }
}
