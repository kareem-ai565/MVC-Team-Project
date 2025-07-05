using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class AvailabilityController : Controller
    {
        private readonly ClinicSystemContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvailabilityController(ClinicSystemContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = _userManager.GetUserId(User);
            if (!int.TryParse(userIdStr, out int parsedUserId))
                return Unauthorized("Invalid user ID.");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == parsedUserId);
            if (doctor == null)
                return Unauthorized("Doctor not found for current user.");

            ViewBag.DoctorId = doctor.Id;


            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CalendarData()
        {
            var userIdStr = _userManager.GetUserId(User);
            if (!int.TryParse(userIdStr, out int parsedUserId))
                return Unauthorized("Invalid user ID.");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == parsedUserId);
            if (doctor == null) return Unauthorized();

            var data = await _context.Availabilities
                .Where(a => a.DoctorId == doctor.Id)
                .ToListAsync();

            var events = data.Select(a => new
            {
                id = a.Id,
                title = a.IsBooked ? "Booked" : "Available",
                start = a.AvailableDate.Add(a.StartTime),
                end = a.EndTime == TimeSpan.Zero
                        ? a.AvailableDate.AddDays(1).Add(a.EndTime)
                        : a.AvailableDate.Add(a.EndTime),
                backgroundColor = a.IsBooked ? "#dc3545" : "#28a745",
                borderColor = a.IsBooked ? "#dc3545" : "#28a745",
                extendedProps = new
                {
                    a.IsBooked,
                    a.SlotDuration,
                    a.MaxPatients
                }
            });

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AvailabilityViewModel dto)
        {
            var start = dto.AvailableDate.Add(dto.StartTime);
            var end = dto.EndTime == TimeSpan.Zero
                ? dto.AvailableDate.AddDays(1).Add(dto.EndTime)
                : dto.AvailableDate.Add(dto.EndTime);

            Console.WriteLine($"Start: {start}, End: {end}, Difference: {(end - start).TotalMinutes}, SlotDuration: {dto.SlotDuration}");

            if (end <= start || (end - start).TotalMinutes % dto.SlotDuration != 0)
                return BadRequest("Invalid time range or slot duration.");

            var entity = new Availability
            {
                DoctorId = dto.DoctorId,
                AvailableDate = dto.AvailableDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SlotDuration = dto.SlotDuration,
                MaxPatients = dto.MaxPatients,
                IsBooked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Availabilities.Add(entity);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Availability created successfully.";
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] AvailabilityViewModel model)
        {
            var availability = await _context.Availabilities.FindAsync(model.Id);
            if (availability == null)
            {
                TempData["ErrorMessage"] = "Availability not found.";
                return NotFound();
            }

            var start = model.AvailableDate.Add(model.StartTime);
            var end = model.EndTime == TimeSpan.Zero
                ? model.AvailableDate.AddDays(1).Add(model.EndTime)
                : model.AvailableDate.Add(model.EndTime);

            Console.WriteLine($"Start: {start}, End: {end}, Difference: {(end - start).TotalMinutes}, SlotDuration: {model.SlotDuration}");

            if (end <= start || (end - start).TotalMinutes % model.SlotDuration != 0)
                return BadRequest("Invalid time range or slot duration.");

            availability.AvailableDate = model.AvailableDate;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;
            availability.SlotDuration = model.SlotDuration;
            availability.MaxPatients = model.MaxPatients;
            availability.UpdatedAt = DateTime.UtcNow;

            _context.Update(availability);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Availability updated successfully.";

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);

            if (availability == null)
            {
                TempData["ErrorMessage"] = "Availability not found.";
                return NotFound();
            }

            if (availability.IsBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete a booked slot.";
                return BadRequest("Cannot delete a booked slot.");
            }
            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Availability deleted successfully.";

            return Ok();
        }
    }
}
