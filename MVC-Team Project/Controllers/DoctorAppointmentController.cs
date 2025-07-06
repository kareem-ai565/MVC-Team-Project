using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;
using System.Linq;
using System.Numerics;

namespace MVC_Team_Project.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorAppointmentController : Controller
{
    private readonly ClinicSystemContext _ctx;
    private readonly UserManager<ApplicationUser> _user;
    private readonly INotificationRepository notification;

    public DoctorAppointmentController(ClinicSystemContext c, UserManager<ApplicationUser> u , INotificationRepository notification)
    {
        _ctx = c;
        _user = u;
        this.notification = notification;
    }

    public async Task<IActionResult> Index()
    {
        int uid = int.Parse(_user.GetUserId(User)!);
        int doctorId = (await _ctx.Doctors.FirstAsync(d => d.UserId == uid)).Id;
        ViewBag.DoctorId = doctorId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CalendarData()
    {
        int uid = int.Parse(_user.GetUserId(User)!);
        int doctorId = (await _ctx.Doctors.FirstAsync(d => d.UserId == uid)).Id;

        var apptEvents = await _ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
            .Select(a => new
            {
                id = a.Id.ToString(),
                title = a.Patient!.User!.FullName,
                start = a.AppointmentDate.Add(a.StartTime),
                end = a.AppointmentDate.Add(a.EndTime),
                backgroundColor = a.Status == "Cancelled" ? "#dc3545"
                                 : a.Status == "Confirmed" ? "#198754"
                                 : "#0d6efd",
                borderColor = "#000000",
                display = "auto",
                allDay = false,
                extendedProps = new
                {
                    a.Status,
                    a.PatientNotes,
                    IsAvailable = false,
                    a.StartTime,
                    a.EndTime
                }
            })
            .ToListAsync();

        var availabilityEvents = await _ctx.Availabilities
            .Where(a => a.DoctorId == doctorId && !a.IsBooked)
            .Select(a => new
            {
                id = $"avail-{a.Id}",
                title = "Available",
                start = a.AvailableDate.Add(a.StartTime),
                end = a.EndTime == TimeSpan.Zero
                            ? a.AvailableDate.AddDays(1)
                            : a.AvailableDate.Add(a.EndTime),
                backgroundColor = "#198754",
                borderColor = "#198754",
                display = "background",
                allDay = a.StartTime == TimeSpan.Zero && a.EndTime == TimeSpan.Zero,
                extendedProps = new
                {
                    Status = "Available",
                    PatientNotes = (string?)null,
                    IsAvailable = true
                }
            })
            .ToListAsync();

        return Json(apptEvents.Cast<object>().Concat(availabilityEvents.Cast<object>()));
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentViewModel vm)
    {
        var appt = await _ctx.Appointments.FindAsync(id);
        if (appt is null) return NotFound();

        if (appt.Status == "Cancelled" || appt.Status == "Completed")
            return BadRequest("Cannot update this appointment.");

        if (appt.Status == "Confirmed")
        {
            Patient patient= await _ctx.Patients.FirstOrDefaultAsync(p=>p.Id == vm.PatientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "patient not found.";
                return BadRequest();
            }
           


            Notification notificate = new Notification()
                {
                    Title = "Appointment",
                    Message = "Appointment has confirmed ",
                    NotificationType = "Appointment",
                    Priority = "high",
                    IsRead = false,
                    UserId = patient.UserId
                };


            await _ctx.Notifications.AddAsync(notificate);

            await _ctx.SaveChangesAsync();

                //notification.Add(notificate);
                //notification.save();

            }
        

        var today = DateTime.Today;
        var apptDate = appt.AppointmentDate.Date;
        if (apptDate < today)
            return BadRequest("Cannot modify past appointments.");

        appt.StartTime = vm.StartTime;
        appt.EndTime = vm.EndTime;

        if (!string.IsNullOrEmpty(vm.Status))
            appt.Status = vm.Status;

        appt.UpdatedAt = DateTime.UtcNow;

        await _ctx.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Cancel(int id)
    {
        var appt = await _ctx.Appointments.FindAsync(id);
        if (appt is null) return NotFound();

        if (appt.Status == "Completed")
            return BadRequest("Cannot cancel a completed appointment.");

        appt.Status = "Cancelled";
        appt.IsDeleted = true;
        appt.DeletedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return Ok();
    }
}
