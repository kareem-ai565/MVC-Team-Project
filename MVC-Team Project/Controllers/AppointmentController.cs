using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentsRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorsRepository _doctorRepository;
        private readonly ISpecialtyRepository _specialtyRepository;

        public AppointmentController(
            IAppointmentsRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorsRepository doctorRepository,
            ISpecialtyRepository specialtyRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _specialtyRepository = specialtyRepository;
        }

        public async Task<IActionResult> Index(string? search, string? status, DateTime? date, int page = 1, int pageSize = 10)
        {
            var appointments = await _appointmentRepository.GetAppointmentsAsync(search, status, date, page, pageSize);

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Date = date;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            ViewBag.StatusOptions = new SelectList(new[]
            {
                new { Value = "", Text = "All Statuses" },
                new { Value = "Scheduled", Text = "Scheduled" },
                new { Value = "Completed", Text = "Completed" },
                new { Value = "Cancelled", Text = "Cancelled" },
                new { Value = "No Show", Text = "No Show" }
            }, "Value", "Text", status);

            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new AppointmentCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var hasConflict = await _appointmentRepository.CheckAppointmentConflictAsync(
                    model.DoctorId, model.AppointmentDate, model.StartTime, model.EndTime);

                if (hasConflict)
                {
                    ModelState.AddModelError("", "The selected time slot conflicts with another appointment.");
                    await PopulateDropdownsAsync();
                    return View(model);
                }

                var result = await _appointmentRepository.CreateAppointmentAsync(model);
                if (result.Success)
                {
                    TempData["Success"] = "Appointment created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            await PopulateDropdownsAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentForEditAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentEditVM model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Invalid appointment ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var result = await _appointmentRepository.UpdateAppointmentAsync(model);
                if (result.Success)
                {
                    TempData["Success"] = "Appointment updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            await PopulateDropdownsAsync();
            return View(model);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["Warning"] = "This appointment is already cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id, string cancellationReason)
        {
            if (string.IsNullOrWhiteSpace(cancellationReason))
            {
                TempData["Error"] = "Cancellation reason is required.";
                return RedirectToAction(nameof(Cancel), new { id });
            }

            var result = await _appointmentRepository.CancelAppointmentAsync(id, cancellationReason);
            if (result.Success)
            {
                TempData["Success"] = "Appointment cancelled successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Cancel), new { id });
            }
        }

        public async Task<IActionResult> Complete(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            if (appointment.Status == "Completed")
            {
                TempData["Warning"] = "This appointment is already completed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var completeVM = new AppointmentCompleteVM
            {
                Id = appointment.Id,
                PatientName = appointment.PatientName,
                DoctorName = appointment.DoctorName,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Symptoms = appointment.Symptoms,
                ConsultationFee = appointment.ConsultationFee
            };

            return View(completeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(AppointmentCompleteVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _appointmentRepository.CompleteAppointmentAsync(model);
                if (result.Success)
                {
                    TempData["Success"] = "Appointment completed successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Calendar(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(selectedDate);

            ViewBag.SelectedDate = selectedDate;
            return View(appointments);
        }

        public async Task<IActionResult> Schedule(int? doctorId, DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var appointments = await _appointmentRepository.GetDoctorScheduleAsync(doctorId, selectedDate);

            ViewBag.SelectedDate = selectedDate;
            ViewBag.DoctorId = doctorId;

            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "Id", "Name", doctorId);

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            try
            {
                var availableSlots = await _appointmentRepository.GetAvailableTimeSlotsAsync(doctorId, date);
                return Json(availableSlots);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            try
            {
                var patients = await _patientRepository.GetAllPatientsAsync();
                var doctors = await _doctorRepository.GetAllDoctorsAsync();
                var specialties = await _specialtyRepository.GetAllSpecialtiesAsync();

                ViewBag.Patients = new SelectList(patients, "Id", "Name");
                ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
                ViewBag.Specialties = new SelectList(specialties, "Id", "Name");

                ViewBag.StatusOptions = new SelectList(new[]
                {
                    new { Value = "Scheduled", Text = "Scheduled" },
                    new { Value = "Completed", Text = "Completed" },
                    new { Value = "Cancelled", Text = "Cancelled" },
                    new { Value = "No Show", Text = "No Show" }
                }, "Value", "Text");
            }
            catch
            {
                ViewBag.Patients = new SelectList(new List<object>(), "Id", "Name");
                ViewBag.Doctors = new SelectList(new List<object>(), "Id", "Name");
                ViewBag.Specialties = new SelectList(new List<object>(), "Id", "Name");
                ViewBag.StatusOptions = new SelectList(new List<object>(), "Value", "Text");
            }
        }
    }
}













//<========================================>//
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using MVC_Team_Project.Models;
//using MVC_Team_Project.View_Models;
//using System.ComponentModel.DataAnnotations;

//namespace MVC_Team_Project.Controllers
//{
//    public class AppointmentController : Controller
//    {
//        private readonly IAppointmentService _appointmentService;
//        private readonly IPatientService _patientService;
//        private readonly IDoctorService _doctorService;
//        private readonly ISpecialtyService _specialtyService;

//        public AppointmentController(
//            IAppointmentService appointmentService,
//            IPatientService patientService,
//            IDoctorService doctorService,
//            ISpecialtyService specialtyService)
//        {
//            _appointmentService = appointmentService;
//            _patientService = patientService;
//            _doctorService = doctorService;
//            _specialtyService = specialtyService;
//        }

//        // GET: Appointment
//        public async Task<IActionResult> Index(string? search, string? status, DateTime? date, int page = 1, int pageSize = 10)
//        {
//            try
//            {
//                var appointments = await _appointmentService.GetAppointmentsAsync(search, status, date, page, pageSize);

//                ViewBag.Search = search;
//                ViewBag.Status = status;
//                ViewBag.Date = date;
//                ViewBag.Page = page;
//                ViewBag.PageSize = pageSize;

//                // Status options for filter dropdown
//                ViewBag.StatusOptions = new SelectList(new[]
//                {
//                    new { Value = "", Text = "All Statuses" },
//                    new { Value = "Scheduled", Text = "Scheduled" },
//                    new { Value = "Completed", Text = "Completed" },
//                    new { Value = "Cancelled", Text = "Cancelled" },
//                    new { Value = "No Show", Text = "No Show" }
//                }, "Value", "Text", status);

//                return View(appointments);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading appointments: " + ex.Message;
//                return View(new List<AppointmentVM>());
//            }
//        }

//        // GET: Appointment/Details/5
//        public async Task<IActionResult> Details(int id)
//        {
//            try
//            {
//                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
//                if (appointment == null)
//                {
//                    TempData["Error"] = "Appointment not found.";
//                    return RedirectToAction(nameof(Index));
//                }

//                return View(appointment);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading appointment details: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // GET: Appointment/Create
//        public async Task<IActionResult> Create()
//        {
//            try
//            {
//                await PopulateDropdownsAsync();
//                return View(new AppointmentCreateVM());
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading create form: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // POST: Appointment/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(AppointmentCreateVM model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    // Check for appointment conflicts
//                    var hasConflict = await _appointmentService.CheckAppointmentConflictAsync(
//                        model.DoctorId, model.AppointmentDate, model.StartTime, model.EndTime);

//                    if (hasConflict)
//                    {
//                        ModelState.AddModelError("", "The selected time slot conflicts with another appointment.");
//                        await PopulateDropdownsAsync();
//                        return View(model);
//                    }

//                    var result = await _appointmentService.CreateAppointmentAsync(model);
//                    if (result.Success)
//                    {
//                        TempData["Success"] = "Appointment created successfully.";
//                        return RedirectToAction(nameof(Index));
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", result.Message);
//                    }
//                }

//                await PopulateDropdownsAsync();
//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error creating appointment: " + ex.Message;
//                await PopulateDropdownsAsync();
//                return View(model);
//            }
//        }

//        // GET: Appointment/Edit/5
//        public async Task<IActionResult> Edit(int id)
//        {
//            try
//            {
//                var appointment = await _appointmentService.GetAppointmentForEditAsync(id);
//                if (appointment == null)
//                {
//                    TempData["Error"] = "Appointment not found.";
//                    return RedirectToAction(nameof(Index));
//                }
//                await PopulateDropdownsAsync();
//                await PopulateDropdownsAsync();
//                return View(appointment);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading appointment for editing: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // POST: Appointment/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, AppointmentEditVM model)
//        {
//            try
//            {
//                if (id != model.Id)
//                {
//                    TempData["Error"] = "Invalid appointment ID.";
//                    return RedirectToAction(nameof(Index));
//                }

//                if (ModelState.IsValid)
//                {
//                    var result = await _appointmentService.UpdateAppointmentAsync(model);
//                    if (result.Success)
//                    {
//                        TempData["Success"] = "Appointment updated successfully.";
//                        return RedirectToAction(nameof(Details), new { id = model.Id });
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", result.Message);
//                    }
//                }

//                await PopulateDropdownsAsync();
//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error updating appointment: " + ex.Message;
//                await PopulateDropdownsAsync();
//                return View(model);
//            }
//        }

//        // GET: Appointment/Cancel/5
//        public async Task<IActionResult> Cancel(int id)
//        {
//            try
//            {
//                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
//                if (appointment == null)
//                {
//                    TempData["Error"] = "Appointment not found.";
//                    return RedirectToAction(nameof(Index));
//                }

//                if (appointment.Status == "Cancelled")
//                {
//                    TempData["Warning"] = "This appointment is already cancelled.";
//                    return RedirectToAction(nameof(Details), new { id });
//                }

//                return View(appointment);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading appointment for cancellation: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // POST: Appointment/Cancel/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CancelConfirmed(int id, string cancellationReason)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(cancellationReason))
//                {
//                    TempData["Error"] = "Cancellation reason is required.";
//                    return RedirectToAction(nameof(Cancel), new { id });
//                }

//                var result = await _appointmentService.CancelAppointmentAsync(id, cancellationReason);
//                if (result.Success)
//                {
//                    TempData["Success"] = "Appointment cancelled successfully.";
//                    return RedirectToAction(nameof(Index));
//                }
//                else
//                {
//                    TempData["Error"] = result.Message;
//                    return RedirectToAction(nameof(Cancel), new { id });
//                }
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error cancelling appointment: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // GET: Appointment/Complete/5
//        public async Task<IActionResult> Complete(int id)
//        {
//            try
//            {
//                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
//                if (appointment == null)
//                {
//                    TempData["Error"] = "Appointment not found.";
//                    return RedirectToAction(nameof(Index));
//                }

//                if (appointment.Status == "Completed")
//                {
//                    TempData["Warning"] = "This appointment is already completed.";
//                    return RedirectToAction(nameof(Details), new { id });
//                }

//                var completeVM = new AppointmentCompleteVM
//                {
//                    Id = appointment.Id,
//                    PatientName = appointment.PatientName,
//                    DoctorName = appointment.DoctorName,
//                    AppointmentDate = appointment.AppointmentDate,
//                    StartTime = appointment.StartTime,
//                    EndTime = appointment.EndTime,
//                    Symptoms = appointment.Symptoms,
//                    ConsultationFee = appointment.ConsultationFee
//                };

//                return View(completeVM);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading appointment for completion: " + ex.Message;
//                return RedirectToAction(nameof(Index));
//            }
//        }

//        // POST: Appointment/Complete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Complete(AppointmentCompleteVM model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    var result = await _appointmentService.CompleteAppointmentAsync(model);
//                    if (result.Success)
//                    {
//                        TempData["Success"] = "Appointment completed successfully.";
//                        return RedirectToAction(nameof(Details), new { id = model.Id });
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", result.Message);
//                    }
//                }

//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error completing appointment: " + ex.Message;
//                return View(model);
//            }
//        }

//        // GET: Appointment/Calendar
//        public async Task<IActionResult> Calendar(DateTime? date)
//        {
//            try
//            {
//                var selectedDate = date ?? DateTime.Today;
//                var appointments = await _appointmentService.GetAppointmentsByDateAsync(selectedDate);

//                ViewBag.SelectedDate = selectedDate;
//                return View(appointments);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading calendar: " + ex.Message;
//                return View(new List<AppointmentVM>());
//            }
//        }

//        // GET: Appointment/Schedule
//        public async Task<IActionResult> Schedule(int? doctorId, DateTime? date)
//        {
//            try
//            {
//                var selectedDate = date ?? DateTime.Today;
//                var appointments = await _appointmentService.GetDoctorScheduleAsync(doctorId, selectedDate);

//                ViewBag.SelectedDate = selectedDate;
//                ViewBag.DoctorId = doctorId;

//                // Load doctors for dropdown
//                var doctors = await _doctorService.GetAllDoctorsAsync();
//                ViewBag.Doctors = new SelectList(doctors, "Id", "Name", doctorId);

//                return View(appointments);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Error loading schedule: " + ex.Message;
//                return View(new List<AppointmentVM>());
//            }
//        }

//        // GET: Appointment/GetAvailableSlots
//        [HttpGet]
//        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
//        {
//            try
//            {
//                var availableSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);
//                return Json(availableSlots);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message });
//            }
//        }

//        // Helper method to populate dropdown lists
//        private async Task PopulateDropdownsAsync()
//        {
//            try
//            {
//                var patients = await _patientService.GetAllPatientsAsync();
//                var doctors = await _doctorService.GetAllDoctorsAsync();
//                var specialties = await _specialtyService.GetAllSpecialtiesAsync();

//                ViewBag.Patients = new SelectList(patients, "Id", "Name");
//                ViewBag.Doctors = new SelectList(doctors, "Id", "Name");
//                ViewBag.Specialties = new SelectList(specialties, "Id", "Name");

//                ViewBag.StatusOptions = new SelectList(new[]
//                {
//                    new { Value = "Scheduled", Text = "Scheduled" },
//                    new { Value = "Completed", Text = "Completed" },
//                    new { Value = "Cancelled", Text = "Cancelled" },
//                    new { Value = "No Show", Text = "No Show" }
//                }, "Value", "Text");
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                ViewBag.Patients = new SelectList(new List<object>(), "Id", "Name");
//                ViewBag.Doctors = new SelectList(new List<object>(), "Id", "Name");
//                ViewBag.Specialties = new SelectList(new List<object>(), "Id", "Name");
//                ViewBag.StatusOptions = new SelectList(new List<object>(), "Value", "Text");
//            }
//        }
//    }

//    // Additional view models for specific operations





//}



















/////////////////










