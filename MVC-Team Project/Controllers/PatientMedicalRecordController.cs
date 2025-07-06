using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using System.Security.Claims;

namespace MVC_Team_Project.Controllers
{
    public class PatientMedicalRecordController : Controller
    {
        private readonly ClinicSystemContext clinicSystem;

        public PatientMedicalRecordController(ClinicSystemContext clinicSystem)
        {
            this.clinicSystem = clinicSystem;
        }
        public IActionResult Index()
        {


            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //string name = User.Identity.Name;

            List<MedicalRecord> medicalRecord = clinicSystem.MedicalRecords.Where(c => c.Patient.UserId == id).ToList();

            return View ("Index", medicalRecord);

            








        }
    }
}
