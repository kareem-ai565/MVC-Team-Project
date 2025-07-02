using System;
using MVC_Team_Project.Models;

namespace MVC_Team_Project.Seeders
{
    public static class DbSeeder
    {
        public static void Seed(ClinicSystemContext context)
        {
            ApplicationUserSeeder.Seed(context);
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
