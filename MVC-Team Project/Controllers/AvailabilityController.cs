using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Controllers
{
    public class AvailabilityController : Controller
    {
        private readonly IAvailabilityRepository _availabilityRepo;

        public AvailabilityController(IAvailabilityRepository availabilityRepo)
        {
            _availabilityRepo = availabilityRepo;
        }

        // List all slots
        public async Task<IActionResult> Index()
        {
            var availabilities = await _availabilityRepo.GetAllAsync();
            return View(availabilities);
        }

        // Slot details
        public async Task<IActionResult> Details(int id)
        {
            var availability = await _availabilityRepo.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return View(availability);
        }

        // Show create form
        public IActionResult Create()
        {
            return View();
        }

        //  Submit new slot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Availability availability)
        {
            if (!ModelState.IsValid) return View(availability);

            var isTaken = await _availabilityRepo.IsSlotTakenAsync(
                availability.DoctorId,
                availability.AvailableDate,
                availability.StartTime);

            if (isTaken)
            {
                ModelState.AddModelError("", "This time slot is already taken.");
                return View(availability);
            }

            availability.CreatedAt = DateTime.Now;
            await _availabilityRepo.AddAsync(availability);
            await _availabilityRepo.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            var availability = await _availabilityRepo.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return View(availability);
        }

        // Submit edits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Availability availability)
        {
            if (!ModelState.IsValid) return View(availability);

            var existing = await _availabilityRepo.GetByIdAsync(availability.Id);
            if (existing == null) return NotFound();

            availability.UpdatedAt = DateTime.Now;
            _availabilityRepo.Update(availability);
            await _availabilityRepo.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //  Confirm delete
        public async Task<IActionResult> Delete(int id)
        {
            var availability = await _availabilityRepo.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return View(availability);
        }

        //  Final delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _availabilityRepo.GetByIdAsync(id);
            if (availability == null) return NotFound();

            _availabilityRepo.Delete(availability);
            await _availabilityRepo.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
