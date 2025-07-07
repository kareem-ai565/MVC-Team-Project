using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using MVC_Team_Project.Services.Auth;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ClinicSystemContext _context;

        public AuthController(IAuthService authService, ClinicSystemContext context)
        {
            _authService = authService;
            _context = context;
        }

        // ======================= Register Patient =======================
        [HttpGet]
        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient(RegisterPatientViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _authService.RegisterPatientAsync(model);
            if (!existingUser.Succeeded)
            {
                foreach (var error in existingUser.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _authService.LoginAsync(new LoginViewModel { Email = model.Email, Password = model.Password });
            return RedirectToAction("Index", "Home");
        }

        // ======================= Register Doctor =======================
        [HttpGet]
        public IActionResult RegisterDoctor()
        {
            ViewBag.Specialties = _context.Specialties.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorViewModel model)
        {
            ViewBag.Specialties = _context.Specialties.ToList();

            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _authService.RegisterDoctorAsync(model);
            if (!existingUser.Succeeded)
            {
                foreach (var error in existingUser.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _authService.LoginAsync(new LoginViewModel { Email = model.Email, Password = model.Password });
            return RedirectToAction("MyPatientsRecords", "MedicalRecord");
        }

        // ======================= Login =======================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _authService.GetUserByEmailAsync(model.Email);
                var roles = await _authService.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Doctors");
                if (roles.Contains("Doctor"))
                    return RedirectToAction("MyPatientsRecords", "MedicalRecord");
                if (roles.Contains("Patient"))
                    return RedirectToAction("Profile", "Patients");

                return RedirectToAction("Index", "Home"); // fallback

            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        
        // ======================= Logout =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ======================= Access Denied =======================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}