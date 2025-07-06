using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Team_Project.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientsController(IPatientRepository patientRepo, IUserRepository userRepo, UserManager<ApplicationUser> userManager)
        {
            _patientRepo = patientRepo;
            _userRepo = userRepo;
            _userManager = userManager;
        }

        // ================= List with pagination
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var patients = await _patientRepo.GetPagedPatientsAsync(page, pageSize);
            var total = await _patientRepo.GetTotalCountAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

            return View(patients);
        }

        // =============== Details
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientRepo.GetPatientWithUserAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // =============== Create (GET)
        // GET: /Patients/Create
        public async Task<IActionResult> Create()
        {
            var availableUsers = await _userRepo.GetUsersWithoutPatientAsync();
            ViewBag.Users = new SelectList(availableUsers, "Id", "FullName");

            return View(new PatientFormVM());
        }

        // POST: /Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientFormVM vm)
        {
            var availableUsers = await _userRepo.GetUsersWithoutPatientAsync();
            ViewBag.Users = new SelectList(availableUsers, "Id", "FullName");

            if (!ModelState.IsValid)
                return View(vm);

            ApplicationUser user;

            // Create new user if no UserId is selected
            if (vm.UserId == null || vm.UserId == 0)
            {
                if (string.IsNullOrWhiteSpace(vm.Password))
                {
                    ModelState.AddModelError("Password", "Password is required when creating a new user.");
                    return View(vm);
                }

                user = new ApplicationUser
                {
                    FullName = $"{vm.FirstName} {vm.LastName}".Trim(),
                    Email = vm.Email,
                    UserName = vm.Email,
                    PhoneNumber = vm.Phone,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    EmailVerified = false
                };

                var result = await _userManager.CreateAsync(user, vm.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return View(vm);
                }
            }
            else
            {
                user = await _userManager.FindByIdAsync(vm.UserId.ToString());
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected user not found.");
                    return View(vm);
                }
            }

            var patient = new Patient
            {
                UserId = user.Id,
                Gender = vm.Gender,
                DOB = vm.DOB,
                Address = vm.Address,
                EmergencyContact = vm.EmergencyContact,
                EmergencyPhone = vm.EmergencyPhone,
                BloodType = vm.BloodType,
                Allergies = vm.Allergies,
                MedicalHistory = vm.MedicalHistory,
                CurrentMedications = vm.CurrentMedications,
                InsuranceProvider = vm.InsuranceProvider,
                InsurancePolicyNumber = vm.InsurancePolicyNumber,
                CreatedAt = DateTime.Now
            };

            await _patientRepo.AddAsync(patient);
            await _patientRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // (Optional) Index and other methods would go here



        // =============== ProfieEdit (GET)
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> ProfileEdit()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _patientRepo.GetByUserIdAsync(user.Id);

            if (patient == null)
                return RedirectToAction("Profile"); // fallback if patient doesn't exist

            var vm = new PatientFormVM
            {
                Id = patient.Id,
                UserId = user.Id,
                FirstName = user.FullName?.Split(' ').FirstOrDefault(),
                LastName = user.FullName?.Split(' ').Skip(1).FirstOrDefault(),
                Email = user.Email,
                Phone = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,

                Gender = patient.Gender,
                DOB = patient.DOB,
                Address = patient.Address,
                EmergencyContact = patient.EmergencyContact,
                EmergencyPhone = patient.EmergencyPhone,
                BloodType = patient.BloodType,
                Allergies = patient.Allergies,
                MedicalHistory = patient.MedicalHistory,
                CurrentMedications = patient.CurrentMedications,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber
            };

            return View("ProfileEdit", vm);
        }



        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(PatientFormVM vm)
        {
            if (!ModelState.IsValid)
                return View("ProfileEdit", vm);

            var user = await _userManager.GetUserAsync(User);
            var patient = await _patientRepo.GetByUserIdAsync(user.Id);

            if (user == null || patient == null)
            {
                TempData["ErrorMessage"] = "Unable to update profile.";
                return RedirectToAction("Profile");
            }

            // Handle profile image upload
            if (vm.ProfileImageFile != null && vm.ProfileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = $"{user.Id}_{Path.GetFileName(vm.ProfileImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await vm.ProfileImageFile.CopyToAsync(fileStream);

                user.ProfilePicture = $"/images/{uniqueFileName}";
            }

            // Update user (AspNetUser) fields
            user.FullName = $"{vm.FirstName} {vm.LastName}".Trim();
            user.Email = vm.Email;
            user.PhoneNumber = vm.Phone;

            // Update patient entity fields
            patient.Gender = vm.Gender;
            patient.DOB = vm.DOB;
            patient.Address = vm.Address;
            patient.EmergencyContact = vm.EmergencyContact;
            patient.EmergencyPhone = vm.EmergencyPhone;
            patient.BloodType = vm.BloodType;
            patient.Allergies = vm.Allergies;
            patient.MedicalHistory = vm.MedicalHistory;
            patient.CurrentMedications = vm.CurrentMedications;
            patient.InsuranceProvider = vm.InsuranceProvider;
            patient.InsurancePolicyNumber = vm.InsurancePolicyNumber;

            // Persist updates
            await _userManager.UpdateAsync(user);
            _patientRepo.Update(patient);
            await _patientRepo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        //=============== Edit (Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientRepo.GetPatientWithUserAsync(id);
            var user = await _userManager.FindByIdAsync(patient.UserId.ToString());

            if (patient == null || user == null) return NotFound();

            var vm = new PatientFormVM
            {
                Id = patient.Id,
                UserId = user.Id,
                FirstName = user.FullName?.Split(' ').FirstOrDefault(),
                LastName = user.FullName?.Split(' ').Skip(1).FirstOrDefault(),
                Email = user.Email,
                Phone = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,

                Gender = patient.Gender,
                DOB = patient.DOB,
                Address = patient.Address,
                EmergencyContact = patient.EmergencyContact,
                EmergencyPhone = patient.EmergencyPhone,
                BloodType = patient.BloodType,
                Allergies = patient.Allergies,
                MedicalHistory = patient.MedicalHistory,
                CurrentMedications = patient.CurrentMedications,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber
            };

            return View(vm); // this should load Views/Patients/Edit.cshtml
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var patient = await _patientRepo.GetByIdAsync(vm.Id.Value);
            var user = await _userManager.FindByIdAsync(vm.UserId.ToString());

            if (patient == null || user == null) return NotFound();

            // Handle profile image upload
            if (vm.ProfileImageFile != null && vm.ProfileImageFile.Length > 0)
            {
                var filename = $"{user.Id}_{Path.GetFileName(vm.ProfileImageFile.FileName)}";
                var filepath = Path.Combine("wwwroot/images", filename);

                using var stream = new FileStream(filepath, FileMode.Create);
                await vm.ProfileImageFile.CopyToAsync(stream);

                user.ProfilePicture = $"/images/{filename}";
            }

            // Update user fields
            user.FullName = $"{vm.FirstName} {vm.LastName}".Trim();
            user.Email = vm.Email;
            user.PhoneNumber = vm.Phone;

            // Update patient fields
            patient.Gender = vm.Gender;
            patient.DOB = vm.DOB;
            patient.Address = vm.Address;
            patient.EmergencyContact = vm.EmergencyContact;
            patient.EmergencyPhone = vm.EmergencyPhone;
            patient.BloodType = vm.BloodType;
            patient.Allergies = vm.Allergies;
            patient.MedicalHistory = vm.MedicalHistory;
            patient.CurrentMedications = vm.CurrentMedications;
            patient.InsuranceProvider = vm.InsuranceProvider;
            patient.InsurancePolicyNumber = vm.InsurancePolicyNumber;

            await _userManager.UpdateAsync(user);
            _patientRepo.Update(patient);
            await _patientRepo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient updated successfully.";
            return RedirectToAction("Index");
        }


        // =============== Delete 
        // =============== Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientRepo.GetPatientWithUserAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient); // View expects a model of type Patient
        }

        // =============== Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Patients/Delete/{id}")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _patientRepo.GetPatientWithUserAsync(id);
            if (patient == null)
                return NotFound();

            var userToDelete = patient.User;

            _patientRepo.Delete(patient);
            await _patientRepo.SaveChangesAsync();

            // Attempt to delete associated user
            if (userToDelete != null)
            {
                var result = await _userManager.DeleteAsync(userToDelete);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    // Optional: re-fetch user for consistency

                    patient.User = await _userManager.FindByIdAsync(userToDelete.Id.ToString());
                    return View("Delete", patient); // show same delete view with errors    
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Profile()
        {
            // Step 1: Get logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            // Step 2: Use existing method by passing user.Id
            var patient = await _patientRepo.GetByUserIdAsync(user.Id);
            if (patient == null)
            {
                TempData["Error"] = "Patient profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Step 3: Map to PatientVM
            var vm = new PatientVM
            {
                Id = patient.Id,
                FullName = user.FullName,
                ProfilePicture = user.ProfilePicture ?? "/images/default-patient.jpg",
                Gender = patient.Gender,
                DOB = patient.DOB,
                Address = patient.Address,
                BloodType = patient.BloodType,
                Allergies = patient.Allergies,
                MedicalHistory = patient.MedicalHistory,
                CurrentMedications = patient.CurrentMedications,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber,
                Appointments = patient.Appointments?.Select(ap => new PatientAppointmentVM
                {
                    AppointmentDate = ap.AppointmentDate,
                    DoctorName = ap.Doctor?.User?.FullName,
                    Status = ap.Status
                }).ToList()
            };

            return View("Profile", vm);
        }




    }
}
