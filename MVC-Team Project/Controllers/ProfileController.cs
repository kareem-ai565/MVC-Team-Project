using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;

namespace MVC_Team_Project.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doctor"))
                return RedirectToAction("Profile", "Doctor");

            if (User.IsInRole("Patient"))
                return RedirectToAction("Profile", "Patient");

            if (User.IsInRole("Admin"))
                return RedirectToAction("Dashboard", "Admin");

            TempData["ErrorMessage"] = "Unknown user role.";
            return RedirectToAction("Index", "Home");
        }
    }
}
