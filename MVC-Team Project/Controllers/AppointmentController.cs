using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC_Team_Project.Controllers
{
    public class AppointmentController : Controller
    {
        ClinicSystemContext context = new ClinicSystemContext();

        //private readonly ClinicSystemContext _context;
        //AppointmentController(ClinicSystemContext context)
        //{

        //    _context = context;


        //}


        public IActionResult Index()
                {
            //var appointments =_context.Appointments.Include(a => a.Doctor).ThenInclude(d => d.User).Include(a => a.Patient).ThenInclude(p => p.User).ToList();

            List<Appointment> appList=   context.Appointments.Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Patient).ThenInclude(p => p.User).ToList();
            //List<Appointment> appList = context.Appointments.ToList();
            //return View(appointments);
            return View("Index", appList);
        }




    }
}
