using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorAppointmentController : Controller
{
    private readonly ClinicSystemContext _ctx;
    private readonly UserManager<ApplicationUser> _user;

    public DoctorAppointmentController(ClinicSystemContext c, UserManager<ApplicationUser> u)
    {
        _ctx = c;
        _user = u;
    }

    /* ---------------------------------------------------- */
    /*  Calendar page                                       */
    /* ---------------------------------------------------- */
    public async Task<IActionResult> Index()
    {
        int uid = int.Parse(_user.GetUserId(User)!);
        int doctorId = (await _ctx.Doctors.FirstAsync(d => d.UserId == uid)).Id;
        ViewBag.DoctorId = doctorId;
        return View();
    }

    /* ---------------------------------------------------- */
    /*  JSON feed for FullCalendar                          */
    /* ---------------------------------------------------- */
    [HttpGet]
    public async Task<IActionResult> CalendarData()
    {
        int uid = int.Parse(_user.GetUserId(User)!);
        int doctorId = (await _ctx.Doctors.FirstAsync(d => d.UserId == uid)).Id;

        /* 1) appointments --------------------------------- */
        var apptEvents = await _ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
            .Select(a => new
            {
                id = a.Id.ToString(),
                title = a.Patient!.User!.FullName,
                start = a.AppointmentDate.Add(a.StartTime),
                end = a.AppointmentDate.Add(a.EndTime),
                backgroundColor = a.Status == "Cancelled" ? "#dc3545" : "#0d6efd",
                borderColor = a.Status == "Cancelled" ? "#dc3545" : "#0d6efd",
                display = "auto",
                allDay = false,
                extendedProps = new { a.Status, a.PatientNotes, IsAvailable = false }
            })
            .ToListAsync();

        /* 2) available slots ------------------------------ */
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
                extendedProps = new { Status = "Available", PatientNotes = (string?)null, IsAvailable = true }
            })
            .ToListAsync();

        /* 3) merge                                         */
        var allEvents = apptEvents.Concat(availabilityEvents).ToList();
        return Json(allEvents);
    }

    /* ---------------------------------------------------- */
    /*  Update                                              */
    /* ---------------------------------------------------- */
    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentViewModel vm)
    {
        var appt = await _ctx.Appointments.FindAsync(id);
        if (appt is null) return NotFound();

        appt.StartTime = vm.StartTime;
        appt.EndTime = vm.EndTime;
        appt.Status = vm.Status;
        appt.UpdatedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return Ok();
    }

    /* ---------------------------------------------------- */
    /*  Cancel                                              */
    /* ---------------------------------------------------- */
    [HttpDelete]
    public async Task<IActionResult> Cancel(int id)
    {
        var appt = await _ctx.Appointments.FindAsync(id);
        if (appt is null) return NotFound();

        appt.Status = "Cancelled";
        appt.IsDeleted = true;
        appt.DeletedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return Ok();
    }
}
