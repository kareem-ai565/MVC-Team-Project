using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ClinicSystemContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ClinicSystemContext context,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterPatientAsync(RegisterPatientViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailVerified = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Patient"))
                    await _roleManager.CreateAsync(new IdentityRole<int>("Patient"));
                await _userManager.AddToRoleAsync(user, "Patient");

                var patient = new Patient
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
            return result;
        }

        public async Task<IdentityResult> RegisterDoctorAsync(RegisterDoctorViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailVerified = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Doctor"))
                    await _roleManager.CreateAsync(new IdentityRole<int>("Doctor"));
                await _userManager.AddToRoleAsync(user, "Doctor");

                var doctor = new Doctor
                {
                    UserId = user.Id,
                    LicenseNumber = model.LicenseNumber,
                    SpecialtyId = model.SpecialtyId.Value,
                    ClinicAddress = model.ClinicAddress,
                    Bio = model.Bio,
                    Education = model.Education,
                    Certifications = model.Certifications,
                    ConsultationFee = model.ConsultationFee.Value,
                    ExperienceYears = model.ExperienceYears.Value,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    _context.Attach(user);
                    user.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            return result;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}