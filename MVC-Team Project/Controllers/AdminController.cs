using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;

namespace MVC_Team_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Profile()
        {
            var admin = _userManager.GetUserAsync(User).Result;
            return View(admin); // model is ApplicationUser
        }

    }
}
