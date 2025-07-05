//using Microsoft.AspNetCore.Mvc;
//using MVC_Team_Project.Models;
//using MVC_Team_Project.View_Models;
//using Microsoft.EntityFrameworkCore;


//namespace MVC_Team_Project.Controllers
//{
//    public class DoctorsController : Controller
//    {
//        private readonly ClinicSystemContext _context;

//        public DoctorsController(ClinicSystemContext context)
//        {
//            _context = context;
//        }

//        // ================= Index with Pagination ====================
//        public IActionResult Index(int page = 1, int pageSize = 5)
//        {
//            var query = _context.Doctors
//                .Include(d => d.User)
//                .Include(d => d.Specialty)
//                .Include(d => d.Appointments).ThenInclude(a => a.Patient).ThenInclude(p => p.User)
//                .Include(d => d.Availabilities);

//            var totalDoctors = query.Count();
//            var doctors = query
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .Select(d => new DoctorsVM
//                {
//                    Id = d.Id,
//                    FullName = d.User.FullName,
//                    SpecialtyName = d.Specialty.Name,
//                    Bio = d.Bio,
//                    ClinicAddress = d.ClinicAddress,
//                    ConsultationFee = d.ConsultationFee,
//                    ExperienceYears = d.ExperienceYears,
//                    IsVerified = d.IsVerified,
//                    Availabilities = d.Availabilities.Select(a => new AvailabilityVM
//                    {
//                        StartTime = a.StartTime,
//                        EndTime = a.EndTime,
//                        SlotDuration = a.SlotDuration,
//                        MaxPatients = a.MaxPatients
//                    }).ToList(),
//                    Appointments = d.Appointments.Select(ap => new DoctorAppointmentVM
//                    {
//                        AppointmentDate = ap.AppointmentDate,
//                        PatientName = ap.Patient.User.FullName,
//                        Status = ap.Status
//                    }).ToList()
//                }).ToList();

//            ViewBag.CurrentPage = page;
//            ViewBag.TotalPages = (int)Math.Ceiling((double)totalDoctors / pageSize);

//            return View(doctors);
//        }

//        // ================= Details ====================
//        public IActionResult Details(int id)
//        {
//            var doctor = _context.Doctors
//                .Include(d => d.User)
//                .Include(d => d.Specialty)
//                .FirstOrDefault(d => d.Id == id);

//            if (doctor == null) return NotFound();

//            return View(doctor);
//        }

//        // ================= Create ====================
//        public IActionResult Create()
//        {
//            ViewBag.Users = _context.Users.ToList();
//            ViewBag.Specialties = _context.Specialties.ToList();
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Create(Doctor doctor)
//        {
//            if (ModelState.IsValid)
//            {
//                doctor.CreatedAt = DateTime.Now;
//                _context.Doctors.Add(doctor);
//                _context.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.Users = _context.Users.ToList();
//            ViewBag.Specialties = _context.Specialties.ToList();
//            return View(doctor);
//        }

//        // ================= Edit ====================
//        public IActionResult Edit(int id)
//        {
//            var doctor = _context.Doctors.Find(id);
//            if (doctor == null) return NotFound();

//            ViewBag.Users = _context.Users.ToList();
//            ViewBag.Specialties = _context.Specialties.ToList();
//            return View(doctor);
//        }

//        [HttpPost]
//        public IActionResult Edit(Doctor doctor)
//        {
//            if (ModelState.IsValid)
//            {
//                doctor.UpdatedAt = DateTime.Now;
//                _context.Doctors.Update(doctor);
//                _context.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.Users = _context.Users.ToList();
//            ViewBag.Specialties = _context.Specialties.ToList();
//            return View(doctor);
//        }

//        // ================= Delete ====================
//        public IActionResult Delete(int id)
//        {
//            var doctor = _context.Doctors
//                .Include(d => d.User)
//                .Include(d => d.Specialty)
//                .FirstOrDefault(d => d.Id == id);

//            if (doctor == null) return NotFound();

//            return View(doctor);
//        }

//        [HttpPost, ActionName("Delete")]
//        public IActionResult DeleteConfirmed(int id)
//        {
//            var doctor = _context.Doctors.Find(id);
//            if (doctor != null)
//            {
//                _context.Doctors.Remove(doctor);
//                _context.SaveChanges();
//            }

//            return RedirectToAction("Index");
//        }
//    }

//}
#region v2

//using Microsoft.AspNetCore.Mvc;
//using MVC_Team_Project.Models;
//using MVC_Team_Project.Repositories.Interfaces;
//using MVC_Team_Project.View_Models;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace MVC_Team_Project.Controllers
//{
//    public class DoctorsController : Controller
//    {
//        private readonly IDoctorsRepository _doctorsRepo;

//        public DoctorsController(IDoctorsRepository doctorsRepo)
//        {
//            _doctorsRepo = doctorsRepo;
//        }

//        // ========== Index with Pagination and Search ==========
//        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 6)
//        {
//            var (data, totalCount) = await _doctorsRepo.GetPagedAsync(search, page, pageSize);

//            var viewModel = data.Select(d => new DoctorsVM
//            {
//                Id = d.Id,
//                FullName = d.User?.FullName,
//                SpecialtyName = d.Specialty?.Name,
//                Bio = d.Bio,
//                ClinicAddress = d.ClinicAddress,
//                ConsultationFee = d.ConsultationFee,
//                ExperienceYears = d.ExperienceYears,
//                IsVerified = d.IsVerified,
//                Availabilities = d.Availabilities?.Select(a => new AvailabilityVM
//                {
//                    StartTime = a.StartTime,
//                    EndTime = a.EndTime,
//                    SlotDuration = a.SlotDuration,
//                    MaxPatients = a.MaxPatients
//                }).ToList(),
//                Appointments = d.Appointments?.Select(ap => new DoctorAppointmentVM
//                {
//                    AppointmentDate = ap.AppointmentDate,
//                    PatientName = ap.Patient?.User?.FullName,
//                    Status = ap.Status
//                }).ToList()
//            }).ToList();

//            ViewBag.CurrentPage = page;
//            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
//            ViewBag.Search = search;

//            return View(viewModel); // ShowAll.cshtml expected
//        }

//        // ========== Details ==========
//        public async Task<IActionResult> Details(int id)
//        {
//            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
//            if (doctor == null) return NotFound();
//            return View(doctor);
//        }

//        // ========== Create ==========
//        // GET
//        public async Task<IActionResult> Create()
//        {
//            var vm = new DoctorFormVM
//            {
//                Users = await _doctorsRepo.GetAvailableUsersAsync(),
//                Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync()
//            };

//            return View(vm);
//        }

//        // POST
//        [HttpPost]
//        public async Task<IActionResult> Create(DoctorFormVM vm)
//        {
//            if (!ModelState.IsValid)
//            {
//                vm.Users = await _doctorsRepo.GetAvailableUsersAsync();
//                vm.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
//                return View(vm);
//            }

//            vm.Doctor.CreatedAt = DateTime.Now;
//            await _doctorsRepo.AddAsync(vm.Doctor);
//            await _doctorsRepo.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        // ========== Edit ==========
//        // GET
//        public async Task<IActionResult> Edit(int id)
//        {
//            var doctor = await _doctorsRepo.GetByIdAsync(id);
//            if (doctor == null) return NotFound();

//            var vm = new DoctorFormVM
//            {
//                Doctor = doctor,
//                Users = await _doctorsRepo.GetAvailableUsersAsync(), // Optional: Add doctor’s current user too
//                Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync()
//            };

//            return View(vm);
//        }

//        // POST
//        [HttpPost]
//        public async Task<IActionResult> Edit(DoctorFormVM vm)
//        {
//            if (!ModelState.IsValid)
//            {
//                vm.Users = await _doctorsRepo.GetAvailableUsersAsync();
//                vm.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
//                return View(vm);
//            }

//            vm.Doctor.UpdatedAt = DateTime.Now;
//            _doctorsRepo.Update(vm.Doctor);
//            await _doctorsRepo.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }



//        // ========== Delete ==========
//        //public async Task<IActionResult> Delete(int id)
//        //{
//        //    var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
//        //    if (doctor == null) return NotFound();
//        //    return View(doctor);
//        //}

//        //[HttpPost, ActionName("Delete")]
//        //public async Task<IActionResult> DeleteConfirmed(int id)
//        //{
//        //    var doctor = await _doctorsRepo.GetByIdAsync(id);
//        //    if (doctor != null)
//        //    {
//        //        _doctorsRepo.Delete(doctor);
//        //        await _doctorsRepo.SaveChangesAsync();
//        //    }
//        //    return RedirectToAction("Index");
//        //}
//        //================== Search ==========
//        [HttpPost, ActionName("Delete")]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);

//            if (doctor == null)
//                return NotFound();

//            if (doctor.Appointments != null && doctor.Appointments.Any())
//            {
//                // Optional: add a message to TempData
//                TempData["Error"] = "Cannot delete doctor with existing appointments.";
//                return RedirectToAction("Index");
//            }

//            _doctorsRepo.Delete(doctor);
//            await _doctorsRepo.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }



//    }
//} 
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

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

            // 1. Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
                return View(model);
            }

            // 2. Add to Doctor Role
            if (!await _roleManager.RoleExistsAsync("Doctor"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Doctor"));

            await _userManager.AddToRoleAsync(user, "Doctor");

            // 3. Create Doctor
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

            //new profile pic upload
            if (model.ProfileImage != null)
            {
                doctor.ProfileImagePath = await SaveProfileImageAsync(model.ProfileImage);
            }

            await _doctorsRepo.AddAsync(doctor);
            await _doctorsRepo.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ===================== Edit Doctor =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
            if (doctor == null) return NotFound();

            var vm = new DoctorFormVM
            {
                Doctor = doctor,
                Users = new List<ApplicationUser> { doctor.User }, // Include current user for info
                Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DoctorFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Users = await _doctorsRepo.GetAvailableUsersAsync();
                vm.Specialties = await _doctorsRepo.GetActiveSpecialtiesAsync();
                return View(vm);
            }

            var doctor = await _doctorsRepo.GetByIdAsync(vm.Doctor.Id);
            if (doctor == null) return NotFound();

            //new profile pic upload
            if (vm.ProfileImage != null)
            {
                doctor.ProfileImagePath = await SaveProfileImageAsync(vm.ProfileImage);
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

            return RedirectToAction("Index");
        }

        // ===================== Delete Doctor =====================
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _doctorsRepo.GetByIdWithDetailsAsync(id);
            if (doctor == null)
                return NotFound();

            if (doctor.Appointments != null && doctor.Appointments.Any())
            {
                TempData["Error"] = "Cannot delete doctor with existing appointments.";
                return RedirectToAction("Index");
            }

            var user = doctor.User;

            // 1. Delete doctor
            _doctorsRepo.Delete(doctor);
            await _doctorsRepo.SaveChangesAsync();

            // 2. Remove roles from user before deleting
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    var roleRemovalResult = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!roleRemovalResult.Succeeded)
                    {
                        TempData["Error"] = "Could not remove user roles before deletion.";
                        return RedirectToAction("Index");
                    }
                }

                var userDeleteResult = await _userManager.DeleteAsync(user);
                if (!userDeleteResult.Succeeded)
                {
                    TempData["Error"] = "Could not delete associated user.";
                    return RedirectToAction("Index");
                }
            }

            TempData["Success"] = "Doctor deleted successfully.";
            return RedirectToAction("Index");
        }
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
                ProfileImagePath = doctor.ProfileImagePath ?? "/images/default-doctor.jpg",
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
        // ===================== Private Utilities =====================
        private async Task<string> SaveProfileImageAsync(IFormFile imageFile)
        {
            if (imageFile == null) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");

            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/profiles/" + fileName;
        }




    }
}

