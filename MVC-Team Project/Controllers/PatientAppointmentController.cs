using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientAppointmentController : Controller
    {
        private readonly ClinicSystemContext _ctx;
        private readonly UserManager<ApplicationUser> _user;
        public PatientAppointmentController(ClinicSystemContext c, UserManager<ApplicationUser> u) { _ctx = c; _user = u; }

        public IActionResult Index(int doctorId) { ViewBag.DoctorId = doctorId; return View(); }

        [HttpGet]
        public async Task<IActionResult> AvailableSlots(int doctorId)
        {
            var slots = await _ctx.Availabilities.Where(a => a.DoctorId == doctorId && !a.IsBooked)
                         .Select(a => new {
                             id = a.Id,
                             start = a.AvailableDate.Add(a.StartTime),
                             end = a.AvailableDate.Add(a.EndTime)
                         }).ToListAsync();
            return Json(slots);
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] AppointmentViewModel vm)
        {
            int uid = int.Parse(_user.GetUserId(User));
            var patient = await _ctx.Patients.FirstAsync(p => p.UserId == uid);
            var avail = await _ctx.Availabilities.FindAsync(vm.AvailabilityId);
            if (avail == null || avail.IsBooked) { TempData["ErrorMessage"] = "Slot not available."; return BadRequest(); }

            var appt = new Appointment
            {
                DoctorId = vm.DoctorId,
                PatientId = patient.Id,
                AvailabilityId = avail.Id,
                AppointmentDate = avail.AvailableDate,
                StartTime = avail.StartTime,
                EndTime = avail.EndTime,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow
            };
            avail.IsBooked = true; _ctx.Add(appt); await _ctx.SaveChangesAsync();
            TempData["SuccessMessage"] = "Appointment booked successfully."; return Ok();
        }
    }
}
