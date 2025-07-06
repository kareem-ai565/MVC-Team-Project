using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using System.Security.Claims;

namespace MVC_Team_Project.Controllers
{
    public class PatientMedicalRecordController : Controller
    {
        private readonly ClinicSystemContext clinicSystemContext;

        public PatientMedicalRecordController(ClinicSystemContext clinicSystemContext)
        {
            this.clinicSystemContext = clinicSystemContext;
        }
        public IActionResult Index()
        {


            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //string name = User.Identity.Name;

            List<MedicalRecord> medicalRecords = clinicSystemContext.MedicalRecords.Where(m => m.Patient.UserId == id).ToList();

            return View(medicalRecords);

        }
    }
}
