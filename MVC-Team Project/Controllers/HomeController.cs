using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClinicSystemContext clinicSystemContext;

        public HomeController(ILogger<HomeController> logger  ,  ClinicSystemContext clinicSystemContext )
        {
            _logger = logger;
            this.clinicSystemContext = clinicSystemContext;
        }


        public IActionResult Index()
        {
          List<RegisterDoctorViewModel> doctors = clinicSystemContext.Doctors.Select(d=>new RegisterDoctorViewModel{
             
              FullName=d.User.FullName,
               ProfilePicturePath=d.User.ProfilePicture,
              SpecialtyId=d.SpecialtyId,
              ClinicAddress=d.ClinicAddress
              


            }).Take(3).ToList();
            ;
            return View("Index", doctors);
        }

        public IActionResult About()
        {
            return View("About");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
