using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorsRepository _doctorsRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DoctorsController(
            IDoctorsRepository doctorsRepo,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _doctorsRepo = doctorsRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ===================== Index =====================
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 6)
        {
            var (data, totalCount) = await _doctorsRepo.GetPagedAsync(search, page, pageSize);

            var viewModel = data.Select(d => new DoctorsVM
            {
                Id = d.Id,
                FullName = d.User?.FullName,
                ProfilePicture = d.User?.ProfilePicture ?? "/images/default-doctor.jpg",
                SpecialtyName = d.Specialty?.Name,
                Bio = d.Bio,
                ClinicAddress = d.ClinicAddress,
                ConsultationFee = d.ConsultationFee,
                ExperienceYears = d.ExperienceYears,
                IsVerified = d.IsVerified,
                Availabilities = d.Availabilities?.Select(a => new DoctorAvailabilityVM
                {
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    SlotDuration = a.SlotDuration,
                    MaxPatients = a.MaxPatients
                }).ToList(),
                Appointments = d.Appointments?.Select(ap => new DoctorAppointmentVM
                {
                    AppointmentDate = ap.AppointmentDate,
                    PatientName = ap.Patient?.User?.FullName,
                    Status = ap.Status
                }).ToList()
            }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;

            return View(viewModel);
        }

        // ===================== Details =====================
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        // ===================== Register Doctor (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
            return View(new RegisterDoctorViewModel());
        }

        // ===================== Register Doctor (POST) =====================
        [HttpPost]
        public async Task<IActionResult> Create(RegisterDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            if (model.ProfilePicture is not null)
                user.ProfilePicture = await SaveProfileImageAsync(model.ProfilePicture);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Doctor"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Doctor"));
            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                UserId = user.Id,
                SpecialtyId = model.SpecialtyId.Value,
                ClinicAddress = model.ClinicAddress,
                Bio = model.Bio,
                LicenseNumber = model.LicenseNumber,
                ConsultationFee = model.ConsultationFee.Value,
                ExperienceYears = model.ExperienceYears.Value,
                Certifications = model.Certifications,
                Education = model.Education,
                CreatedAt = DateTime.Now,
                IsVerified = model.IsVerified
            };

            await _doctorsRepo.AddAsync(doctor);
            await _doctorsRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== Edit Doctor (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
            if (doctor == null) return NotFound();

            var vm = new DoctorFormVM
            {
                Doctor = doctor,
                Users = new List<ApplicationUser> { doctor.User },
                Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync()
            };

            return View(vm);
        }

        // ===================== Edit Doctor (POST) =====================
        [HttpPost]
        public async Task<IActionResult> Edit(DoctorFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Users = await _doctorsRepo.GetAvailableUsersAsync();
                vm.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
                return View(vm);
            }

            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(vm.Doctor.Id);
            if (doctor is null) return NotFound();

            if (vm.ProfilePicture is not null)
            {
                doctor.User.ProfilePicture = await SaveProfileImageAsync(vm.ProfilePicture);
                doctor.User.UpdatedAt = DateTime.Now;
            }

            doctor.SpecialtyId = vm.Doctor.SpecialtyId;
            doctor.ClinicAddress = vm.Doctor.ClinicAddress;
            doctor.Bio = vm.Doctor.Bio;
            doctor.LicenseNumber = vm.Doctor.LicenseNumber;
            doctor.ConsultationFee = vm.Doctor.ConsultationFee;
            doctor.ExperienceYears = vm.Doctor.ExperienceYears;
            doctor.Certifications = vm.Doctor.Certifications;
            doctor.Education = vm.Doctor.Education;
            doctor.UpdatedAt = DateTime.Now;
            doctor.IsVerified = vm.Doctor.IsVerified;

            _doctorsRepo.Update(doctor);
            await _doctorsRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== Delete Doctor =====================
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
            if (doctor == null) return NotFound();

            if (doctor.Appointments?.Any() == true)
            {
                TempData["Error"] = "Cannot delete doctor with existing appointments.";
                return RedirectToAction("Index");
            }

            var user = doctor.User;

            _doctorsRepo.Delete(doctor);
            await _doctorsRepo.SaveChangesAsync();

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    var roleRemoval = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!roleRemoval.Succeeded)
                    {
                        TempData["Error"] = "Could not remove user roles before deletion.";
                        return RedirectToAction("Index");
                    }
                }

                var userDelete = await _userManager.DeleteAsync(user);
                if (!userDelete.Succeeded)
                {
                    TempData["Error"] = "Could not delete associated user.";
                    return RedirectToAction("Index");
                }
            }

            TempData["Success"] = "Doctor deleted successfully.";
            return RedirectToAction("Index");
        }

        // ===================== Doctor Profile =====================
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var doctor = await _doctorsRepo.GetByUserIdWithDetailsAsync(user.Id);
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var vm = new DoctorsVM
            {
                Id = doctor.Id,
                FullName = user.FullName,
                ProfilePicture = user.ProfilePicture ?? "/images/default-doctor.jpg",
                SpecialtyName = doctor.Specialty?.Name,
                ClinicAddress = doctor.ClinicAddress,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                ExperienceYears = doctor.ExperienceYears,
                IsVerified = doctor.IsVerified,
                Availabilities = doctor.Availabilities?.Select(a => new DoctorAvailabilityVM
                {
                    AvailableDate = a.AvailableDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    SlotDuration = a.SlotDuration,
                    MaxPatients = a.MaxPatients,
                    IsBooked = a.IsBooked
                }).ToList(),
                Appointments = doctor.Appointments?.Select(ap => new DoctorAppointmentVM
                {
                    AppointmentDate = ap.AppointmentDate,
                    PatientName = ap.Patient?.User?.FullName,
                    Status = ap.Status
                }).ToList()
            };

            return View(vm);
        }

        // ===================== Save Profile Picture =====================
        private async Task<string> SaveProfileImageAsync(IFormFile imageFile)
        {
            if (imageFile == null) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/images/profiles/" + fileName;
        }
    }
}
