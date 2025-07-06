using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Implementations;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC_Team_Project.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientAppointmentController : Controller
    {
        private readonly ClinicSystemContext _ctx;
        private readonly UserManager<ApplicationUser> _user;
        private readonly INotificationRepository notification;

        public PatientAppointmentController(ClinicSystemContext ctx, UserManager<ApplicationUser> user , INotificationRepository notification)
        {
            _ctx = ctx;
            _user = user;
            this.notification = notification;
        }

        public IActionResult Index(int doctorId)
        {
            ViewBag.DoctorId = doctorId;
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AvailableTimeBlocks(int doctorId, DateTime start, DateTime end)
        {
            var availabilities = await _ctx.Availabilities
                .Where(a => a.DoctorId == doctorId &&
                            a.AvailableDate >= start &&
                            a.AvailableDate <= end)
                .ToListAsync();

            var events = new List<object>();
            var greenDays = new HashSet<DateTime>();

            foreach (var avail in availabilities)
            {
                int slotSize = avail.SlotDuration > 0 ? avail.SlotDuration : 30;
                DateTime cur = avail.AvailableDate.Add(avail.StartTime);
                DateTime endTime = avail.AvailableDate.Add(avail.EndTime);

                while (cur + TimeSpan.FromMinutes(slotSize) <= endTime)
                {
                    DateTime next = cur.AddMinutes(slotSize);

                    bool isBooked = await _ctx.Appointments.AnyAsync(ap =>
                        ap.AvailabilityId == avail.Id &&
                        ap.StartTime == cur.TimeOfDay &&
                        ap.EndTime == next.TimeOfDay &&
                        !ap.IsDeleted &&
                        ap.Status != "Cancelled");

                    if (!isBooked && cur >= DateTime.Now)
                    {
                        events.Add(new
                        {
                            start = cur.ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = next.ToString("yyyy-MM-ddTHH:mm:ss"),
                            display = "background",
                            color = "#ccffcc"
                        });

                        greenDays.Add(avail.AvailableDate.Date);
                    }

                    cur = next;
                }
            }

            foreach (var day in greenDays)
            {
                events.Add(new
                {
                    start = day.ToString("yyyy-MM-dd"),
                    end = day.ToString("yyyy-MM-dd"),
                    display = "background",
                    color = "#eaffea"
                });
            }

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> BookedSlots(int doctorId, DateTime start, DateTime end)
        {
            var booked = await _ctx.Appointments
                .Where(ap => ap.DoctorId == doctorId &&
                             ap.AppointmentDate >= start &&
                             ap.AppointmentDate <= end &&
                             !ap.IsDeleted &&
                             ap.Status != "Cancelled")
                .Select(ap => new
                {
                    start = ap.AppointmentDate.Add(ap.StartTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = ap.AppointmentDate.Add(ap.EndTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                    display = "background",
                    color = "#ff9999",
                    isBooked = true
                })
                .ToListAsync();

            return Json(booked);
        }

        [HttpGet]
        public async Task<IActionResult> SlotsByDate(int doctorId, DateTime date)
        {
            var availabilities = await _ctx.Availabilities
                .Where(a => a.DoctorId == doctorId &&
                            a.AvailableDate.Date == date.Date)
                .ToListAsync();

            var result = new List<object>();

            foreach (var avail in availabilities)
            {
                int slotSize = avail.SlotDuration > 0 ? avail.SlotDuration : 30;
                DateTime cur = avail.AvailableDate.Add(avail.StartTime);
                DateTime end = avail.AvailableDate.Add(avail.EndTime);

                while (cur + TimeSpan.FromMinutes(slotSize) <= end)
                {
                    DateTime next = cur.AddMinutes(slotSize);

                    bool isBooked = await _ctx.Appointments.AnyAsync(ap =>
                        ap.AvailabilityId == avail.Id &&
                        ap.StartTime == cur.TimeOfDay &&
                        ap.EndTime == next.TimeOfDay &&
                        !ap.IsDeleted &&
                        ap.Status != "Cancelled");

                    result.Add(new
                    {
                        availabilityId = avail.Id,
                        start = cur,
                        end = next,
                        isBooked,
                        isPast = cur < DateTime.Now
                    });

                    cur = next;
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] AppointmentViewModel vm)
        {
            int uid = int.Parse(_user.GetUserId(User)!);
            var patient = await _ctx.Patients.FirstOrDefaultAsync(p => p.UserId == uid);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return BadRequest();
            }

            var avail = await _ctx.Availabilities.FindAsync(vm.AvailabilityId);
            if (avail == null)
            {
                TempData["ErrorMessage"] = "Availability not found.";
                return BadRequest();
            }

            DateTime startDateTime = vm.AppointmentDate.Date + vm.StartTime;
            if (startDateTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Cannot book a slot in the past.";
                return BadRequest();
            }

            bool already = await _ctx.Appointments.AnyAsync(a =>
                a.AvailabilityId == vm.AvailabilityId &&
                a.StartTime == vm.StartTime &&
                a.EndTime == vm.EndTime &&
                !a.IsDeleted &&
                a.Status != "Cancelled");

            if (already)
            {
                TempData["ErrorMessage"] = "This time slot is already booked.";
                return BadRequest();
            }

            var appt = new Appointment
            {
                DoctorId = vm.DoctorId,
                PatientId = patient.Id,
                AvailabilityId = avail.Id,
                AppointmentDate = vm.AppointmentDate,
                StartTime = vm.StartTime,
                EndTime = vm.EndTime,
                Status = "Pending",
                PatientNotes = vm.PatientNotes,
                CreatedAt = DateTime.UtcNow
            };

            _ctx.Appointments.Add(appt);
            await _ctx.SaveChangesAsync();


            var doctor = await _ctx.Doctors.FirstOrDefaultAsync(d => d.Id == vm.DoctorId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return BadRequest();
            }



            Notification notificate = new Notification()
            {
                Title = "Appointment",
                Message = "I book Appointment with u",
                NotificationType = "Appointment",
                Priority = "Low",
                IsRead = false,
                UserId = doctor.UserId
            };
            notification.Add(notificate);
            notification.save();




            TempData["SuccessMessage"] = "The booking has been sent and is awaiting confirmation.";
            return Ok();
        }
    }
}
