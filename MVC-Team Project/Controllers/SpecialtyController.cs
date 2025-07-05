using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;


namespace MVC_Team_Project.Controllers
{
    public class SpecialtyController : Controller
    {
        private readonly ISpecialtyRepository _specialtyRepo;

        public SpecialtyController(ISpecialtyRepository specialtyRepo)
        {
            _specialtyRepo = specialtyRepo;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var specialties = await _specialtyRepo.GetAllAsync();
        //    return View(specialties);
        //}

        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 8)
        {
            var (data, totalCount) = await _specialtyRepo.GetPagedAsync(search, page, pageSize);

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(data);
        }


        public async Task<IActionResult> Active()
        {
            var activeSpecialties = await _specialtyRepo.GetActiveSpecialtiesAsync();
            return View(activeSpecialties);
        }

        public async Task<IActionResult> Details(int id)
        {
            var specialty = await _specialtyRepo.GetByIdAsync(id);
            if (specialty == null) return NotFound();
            return View(specialty);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Specialty specialty)
        {
            if (!ModelState.IsValid) return View(specialty);

            specialty.CreatedAt = DateTime.UtcNow;
            await _specialtyRepo.AddAsync(specialty);
            await _specialtyRepo.SaveChangesAsync();
            TempData["SuccessMessage"] = "Specialty created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var specialty = await _specialtyRepo.GetByIdAsync(id);
            if (specialty == null) return NotFound();
            return View(specialty);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Specialty specialty)
        {
            if (!ModelState.IsValid) return View(specialty);

            _specialtyRepo.Update(specialty);
            await _specialtyRepo.SaveChangesAsync();
            TempData["SuccessMessage"] = "Specialty updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var specialty = await _specialtyRepo.GetByIdAsync(id);
                if (specialty == null)
                {
                    TempData["ErrorMessage"] = "Specialty not found.";
                    return RedirectToAction(nameof(Index));
                }

                _specialtyRepo.Delete(specialty);
                await _specialtyRepo.SaveChangesAsync();

                TempData["SuccessMessage"] = "Specialty deleted successfully.";
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("FK_Doctors_Specialties") == true)
            {
                TempData["ErrorMessage"] = "Can't delete this specialty because there are doctors assigned to it.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting.";
            }

            return RedirectToAction(nameof(Index));
        }
        //==============================================================================================================///
        // additions made by kareem to use in new views to show specialities for users

        public async Task<IActionResult> ShowAll()
        {
            var specialties = await _specialtyRepo.GetActiveSpecialtiesAsync();
            return View(specialties); 

        }
        public async Task<IActionResult> ShowAllDoctorsInSpecialty(int id)
        {
            var viewModel = await _specialtyRepo.GetSpecialtyWithDoctorsAsync(id);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Specialty not found.";
                return RedirectToAction(nameof(ShowAll)); // fallback to specialties view
            }

            return View(viewModel); // This will use Views/Specialty/ShowAllDoctorsInSpecialty.cshtml
        }





    }
}
