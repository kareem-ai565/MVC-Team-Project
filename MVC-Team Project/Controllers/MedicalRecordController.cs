using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    [Authorize]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClinicSystemContext _context;

        public MedicalRecordController(
            IMedicalRecordRepository repo,
            UserManager<ApplicationUser> userManager,
            ClinicSystemContext context)
        {
            _repo = repo;
            _userManager = userManager;
            _context = context;
        }

        // ========== For Patients ==========
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyRecords()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index", "Home");
            }

            var records = await _repo.GetByPatientIdAsync(patient.Id);
            var viewModel = records.Select(r => new MedicalRecordViewModel
            {
                Id = r.Id,
                RecordType = r.RecordType,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                Prescription = r.Prescription,
                TestResults = r.TestResults,
                Notes = r.Notes,
                RecordDate = r.RecordDate,
                IsConfidential = r.IsConfidential,
                DoctorName = r.Doctor?.User?.FullName
            });

            return View(viewModel);
        }

        // ========== For Doctors ==========
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyPatientsRecords()
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction("Index", "Home");
            }

            var records = await _repo.GetByDoctorIdAsync(doctor.Id);
            var viewModel = records.Select(r => new MedicalRecordViewModel
            {
                Id = r.Id,
                RecordType = r.RecordType,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                Prescription = r.Prescription,
                TestResults = r.TestResults,
                Notes = r.Notes,
                RecordDate = r.RecordDate,
                IsConfidential = r.IsConfidential,
                PatientName = r.Patient?.User?.FullName
            });

            return View(viewModel);
        }

        // ========== Create ==========
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            ViewBag.Patients = await _context.Patients
                .Include(p => p.User)
                .Select(p => new { p.Id, Name = p.User.FullName })
                .ToListAsync();

            var viewModel = new MedicalRecordViewModel
            {
                DoctorId = doctor.Id // مهم جداً لتفادي خطأ Required
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _context.Patients
                    .Include(p => p.User)
                    .Select(p => new { p.Id, Name = p.User.FullName })
                    .ToListAsync();
                model.DoctorId = doctor.Id; // إعادة تعبئة DoctorId عند العودة للـ form
                return View(model);
            }

            var record = new MedicalRecord
            {
                PatientId = model.PatientId,
                DoctorId = doctor.Id,
                RecordType = model.RecordType,
                Diagnosis = model.Diagnosis,
                Treatment = model.Treatment,
                Prescription = model.Prescription,
                TestResults = model.TestResults,
                Notes = model.Notes,
                RecordDate = model.RecordDate,
                IsConfidential = model.IsConfidential,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(record);
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical record added successfully.";
            return RedirectToAction(nameof(MyPatientsRecords));
        }

        // ========== Edit ==========
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Record not found.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (record.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this record.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            ViewBag.Patients = await _context.Patients
                .Include(p => p.User)
                .Select(p => new { p.Id, Name = p.User.FullName })
                .ToListAsync();

            var viewModel = new MedicalRecordViewModel
            {
                Id = record.Id,
                PatientId = record.PatientId,
                DoctorId = record.DoctorId,
                RecordType = record.RecordType,
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                Prescription = record.Prescription,
                TestResults = record.TestResults,
                Notes = record.Notes,
                RecordDate = record.RecordDate,
                IsConfidential = record.IsConfidential,
                DoctorName = record.Doctor?.User?.FullName,
                PatientName = record.Patient?.User?.FullName
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalRecordViewModel model)
        {
            var record = await _repo.GetByIdAsync(model.Id);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Record not found.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (record.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this record.";
                return RedirectToAction(nameof(MyPatientsRecords));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _context.Patients
                    .Include(p => p.User)
                    .Select(p => new { p.Id, Name = p.User.FullName })
                    .ToListAsync();
                model.DoctorId = doctor.Id;
                return View(model);
            }

            record.RecordType = model.RecordType;
            record.Diagnosis = model.Diagnosis;
            record.Treatment = model.Treatment;
            record.Prescription = model.Prescription;
            record.TestResults = model.TestResults;
            record.Notes = model.Notes;
            record.IsConfidential = model.IsConfidential;
            record.RecordDate = model.RecordDate;
            record.UpdatedAt = DateTime.UtcNow;

            _repo.Update(record);
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical record updated successfully.";
            return RedirectToAction(nameof(MyPatientsRecords));
        }

        // ========== Delete ==========
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var record = await _repo.GetByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Medical record not found.";
                    return RedirectToAction(nameof(MyPatientsRecords));
                }

                var user = await _userManager.GetUserAsync(User);
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

                if (record.DoctorId != doctor.Id)
                {
                    TempData["ErrorMessage"] = "You are not authorized to delete this record.";
                    return RedirectToAction(nameof(MyPatientsRecords));
                }

                _repo.SoftDelete(record, user.Id);
                await _repo.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medical record deleted successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the record.";
            }

            return RedirectToAction(nameof(MyPatientsRecords));
        }
    }
}
