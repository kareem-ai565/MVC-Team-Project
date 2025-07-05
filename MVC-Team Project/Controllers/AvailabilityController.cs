using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class AvailabilityController : Controller
    {
        private readonly IAvailabilityRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClinicSystemContext _context;

        public AvailabilityController(IAvailabilityRepository repo, UserManager<ApplicationUser> userManager, ClinicSystemContext context)
        {
            _repo = repo;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction("Index", "Home");
            }

            var availabilities = await _repo.GetByDoctorIdAsync(doctor.Id);
            var viewModel = availabilities.Select(a => new AvailabilityViewModel
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                AvailableDate = a.AvailableDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                SlotDuration = a.SlotDuration,
                MaxPatients = a.MaxPatients,
                IsBooked = a.IsBooked
            });

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AvailabilityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction(nameof(Index));
            }

            var availability = new Availability
            {
                DoctorId = doctor.Id,
                AvailableDate = model.AvailableDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                SlotDuration = model.SlotDuration,
                MaxPatients = model.MaxPatients,
                IsBooked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(availability);
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Availability added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var availability = await _repo.GetByIdAsync(id);
            if (availability == null)
            {
                TempData["ErrorMessage"] = "Slot not found.";
                return RedirectToAction(nameof(Index));
            }

            _repo.Delete(availability);
            await _repo.SaveChangesAsync();
            TempData["SuccessMessage"] = "Availability deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var availability = await _repo.GetByIdAsync(id);
            if (availability == null)
            {
                TempData["ErrorMessage"] = "Slot not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AvailabilityViewModel
            {
                Id = availability.Id,
                DoctorId = availability.DoctorId,
                AvailableDate = availability.AvailableDate,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                SlotDuration = availability.SlotDuration,
                MaxPatients = availability.MaxPatients
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var availability = await _repo.GetByIdAsync(model.Id);
            if (availability == null)
            {
                TempData["ErrorMessage"] = "Slot not found.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null || availability.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "Unauthorized action.";
                return RedirectToAction(nameof(Index));
            }

            availability.AvailableDate = model.AvailableDate;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;
            availability.SlotDuration = model.SlotDuration;
            availability.MaxPatients = model.MaxPatients;
            availability.UpdatedAt = DateTime.UtcNow;

            _repo.Update(availability);
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Availability updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
