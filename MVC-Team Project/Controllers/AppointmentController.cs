using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;


namespace MVC_Team_Project.Controllers
{

        [Authorize]
        public class AppointmentController : Controller
        {
            private readonly IAppointmentRepository _repo;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ClinicSystemContext _ctx;

            public AppointmentController(IAppointmentRepository repo,
                                         UserManager<ApplicationUser> userManager,
                                         ClinicSystemContext ctx)
            {
                _repo = repo;
                _userManager = userManager;
                _ctx = ctx;
            }

            /*───────────────────── 1. Doctor Calendar ─────────────────────*/
            [Authorize(Roles = "Doctor")]
            public async Task<IActionResult> DoctorCalendar()
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _ctx.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor is null) return RedirectToAction("Index", "Home");

                ViewBag.DoctorName = doctor.User.FullName;
                return View();                          // Views/Appointment/DoctorCalendar
            }

            /* Fetch busy dates JSON */
            [Authorize(Roles = "Doctor")]
            public async Task<IActionResult> GetDoctorBusyDates()
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _ctx.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor is null) return Json(Array.Empty<DateTime>());

                var dates = await _repo.GetAvailableDatesAsync(doctor.Id);
                return Json(dates);
            }

            /* Fetch appointments of a date */
            [Authorize(Roles = "Doctor")]
            public async Task<IActionResult> DoctorDayAppointments(DateTime date)
            {
                var user = await _userManager.GetUserAsync(User);
                var doctor = await _ctx.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor is null) return PartialView("_PatientDayAppointments", Enumerable.Empty<AppointmentViewModel>());

                var list = await _repo.GetByDoctorOnDateAsync(doctor.Id, date);
                var vm = list.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientName = a.Patient.User.FullName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status
                });
                return PartialView("_PatientDayAppointments", vm);
            }

            /*───────────────────── 2. Patient Calendar ───────────────────*/
            [Authorize(Roles = "Patient")]
            public async Task<IActionResult> PatientCalendar()
            {
                var doctors = await _ctx.Doctors
                                        .Include(d => d.User)
                                        .Where(d => d.IsVerified)
                                        .Select(d => new { d.Id, d.User.FullName })
                                        .ToListAsync();

                ViewBag.Doctors = doctors;              // dropdown 
                return View();                          // Views/Appointment/PatientCalendar
            }

            [Authorize(Roles = "Patient")]
            public async Task<IActionResult> GetBusyDates(int doctorId)
            {
                var dates = await _repo.GetAvailableDatesAsync(doctorId);
                return Json(dates);
            }

            [Authorize(Roles = "Patient")]
            public async Task<IActionResult> GetSlots(int doctorId, DateTime date)
            {
                var slots = await _repo.GetByDoctorOnDateAsync(doctorId, date);
                var vm = slots.Select(a => new
                {
                    a.Id,
                    start = a.StartTime.ToString(@"hh\:mm"),
                    end = a.EndTime.ToString(@"hh\:mm"),
                    a.Status
                });
                return Json(vm);
            }

            /*  (GET) */
            [Authorize(Roles = "Patient")]
            public async Task<IActionResult> BookModal(int appointmentId)
            {
                var appt = await _repo.GetByIdAsync(appointmentId);
                if (appt is null || appt.Status != "Available") return StatusCode(404);

                var vm = new AppointmentViewModel
                {
                    Id = appt.Id,
                    DoctorName = appt.Doctor.User.FullName,
                    AppointmentDate = appt.AppointmentDate,
                    StartTime = appt.StartTime,
                    EndTime = appt.EndTime
                };
                return PartialView("_BookAppointmentModal", vm);
            }

            /*  (POST) */
            [Authorize(Roles = "Patient")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Book(AppointmentViewModel vm)
            {
                var appt = await _repo.GetByIdAsync(vm.Id);
                if (appt is null || appt.Status != "Available")
                    return Json(new { success = false, message = "Slot no longer available" });

                var user = await _userManager.GetUserAsync(User);
                var patient = await _ctx.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (patient is null)
                    return Json(new { success = false, message = "Patient profile missing" });

                appt.PatientId = patient.Id;
                appt.Status = "Booked";
                appt.PatientNotes = vm.PatientNotes;
                appt.Symptoms = vm.Symptoms;
                appt.UpdatedAt = DateTime.UtcNow;

                _repo.Update(appt);
                await _repo.SaveChangesAsync();

                return Json(new { success = true });
            }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appt = await _repo.GetByIdAsync(id);
            if (appt == null || appt.Status != "Available")
                return Json(new { success = false, message = "Slot not found or already booked." });

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _ctx.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null || appt.DoctorId != doctor.Id)
                return Json(new { success = false, message = "Unauthorized action." });

            _repo.Delete(appt);
            await _repo.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}