
using MVC_Team_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class AppointmentsRepository : IAppointmentsRepository
    {
        private readonly ClinicSystemContext _context;

        public AppointmentsRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentVM>> GetAppointmentsAsync(string search, string status, DateTime? date, int page, int pageSize)
        {
            var query = _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.Patient.User.FullName.Contains(search) ||
                    a.Doctor.User.FullName.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate == date.Value.Date);
            }

            return await query
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentVM
                {
                    Id = a.Id,
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Symptoms = a.Symptoms,
                    PatientNotes = a.PatientNotes,
                    //ConsultationFee = a.ConsultationFee
                })
                .ToListAsync();
        }

        public async Task<AppointmentVM> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return null;

            return new AppointmentVM
            {
                Id = appointment.Id,
                PatientName = appointment.Patient.User.FullName,
                DoctorName = appointment.Doctor.User.FullName,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Symptoms = appointment.Symptoms,
                PatientNotes = appointment.PatientNotes,
                //ConsultationFee = appointment.ConsultationFee
            };
        }

        public async Task<AppointmentEditVM> GetAppointmentForEditAsync(int id)
        {
            var a = await _context.Appointments.FindAsync(id);
            if (a == null) return null;

            return new AppointmentEditVM
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Symptoms = a.Symptoms,
                PatientNotes = a.PatientNotes,
                //ConsultationFee = a.ConsultationFee
            };
        }

        public async Task<(bool Success, string Message)> CreateAppointmentAsync(AppointmentCreateVM model)
        {
            try
            {
                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = "Scheduled",
                    Symptoms = model.Symptoms,
                    PatientNotes = model.PatientNotes,
                    //ConsultationFee = model.ConsultationFee,
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return (true, "Created successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAppointmentAsync(AppointmentEditVM model)
        {
            var appointment = await _context.Appointments.FindAsync(model.Id);
            if (appointment == null) return (false, "Not found");

            appointment.PatientId = model.PatientId;
            appointment.DoctorId = model.DoctorId;
            appointment.AppointmentDate = model.AppointmentDate.Date;
            appointment.StartTime = model.StartTime;
            appointment.EndTime = model.EndTime;
            appointment.Status = model.Status;
            appointment.Symptoms = model.Symptoms;
            appointment.PatientNotes = model.PatientNotes;
            //appointment.ConsultationFee = model.ConsultationFee;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return (true, "Updated successfully");
        }

        public async Task<(bool Success, string Message)> CancelAppointmentAsync(int id, string reason)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return (false, "Not found");

            appointment.Status = "Cancelled";
            appointment.CancellationReason = reason;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return (true, "Cancelled successfully");
        }

        public async Task<(bool Success, string Message)> CompleteAppointmentAsync(AppointmentCompleteVM model)
        {
            var appointment = await _context.Appointments.FindAsync(model.Id);
            if (appointment == null) return (false, "Not found");

            appointment.Status = "Completed";
            appointment.Diagnosis = model.Diagnosis;
            appointment.DoctorNotes = model.DoctorNotes;
            appointment.Prescription = model.Prescription;
            appointment.IsFollowUpRequired = model.IsFollowUpRequired;
            appointment.FollowUpDate = model.FollowUpDate;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return (true, "Completed successfully");
        }

        public async Task<bool> CheckAppointmentConflictAsync(int doctorId, DateTime date, TimeSpan start, TimeSpan end)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == date.Date &&
                ((start >= a.StartTime && start < a.EndTime) ||
                 (end > a.StartTime && end <= a.EndTime) ||
                 (start <= a.StartTime && end >= a.EndTime))
            );
        }

        public async Task<List<AppointmentVM>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate == date)
                .Select(a => new AppointmentVM
                {
                    Id = a.Id,
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Symptoms = a.Symptoms,
                    PatientNotes = a.PatientNotes,
                    //ConsultationFee = a.ConsultationFee
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentVM>> GetDoctorScheduleAsync(int? doctorId, DateTime date)
        {
            var query = _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate == date);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            return await query.Select(a => new AppointmentVM
            {
                Id = a.Id,
                PatientName = a.Patient.User.FullName,
                DoctorName = a.Doctor.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Symptoms = a.Symptoms,
                PatientNotes = a.PatientNotes,
                //ConsultationFee = a.ConsultationFee
            }).ToListAsync();
        }

        public async Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var allSlots = Enumerable.Range(9, 9)
                .Select(h => new TimeSpan(h, 0, 0)) // 09:00 إلى 17:00
                .ToList();

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .Select(a => a.StartTime)
                .ToListAsync();

            return allSlots.Except(bookedSlots).ToList();
        }
    }
}












///////////////

//using Microsoft.EntityFrameworkCore;
//using MVC_Team_Project.Models;
//using MVC_Team_Project.Repositories.Interfaces;

//namespace MVC_Team_Project.Repositories
//{
//    public class AppointmentsRepository : IAppointmentsRepository
//    {
//        private readonly ClinicSystemContext _context;

//        public AppointmentsRepository(ClinicSystemContext context)
//        {
//            _context = context;
//        }

//        // Basic CRUD operations
//        public async Task<IEnumerable<Appointment>> GetAllAsync()
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Include(a => a.Availability)
//                .Where(a => !a.IsDeleted)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<Appointment?> GetByIdAsync(int id)
//        {
//            return await _context.Appointments
//                .Where(a => a.Id == id && !a.IsDeleted)
//                .FirstOrDefaultAsync();
//        }

//        public async Task<Appointment?> GetByIdWithDetailsAsync(int id)
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Include(a => a.Availability)
//                .Include(a => a.MedicalRecords)
//                .Include(a => a.Payments)
//                .Include(a => a.DeletedByNavigation)
//                .Where(a => a.Id == id && !a.IsDeleted)
//                .FirstOrDefaultAsync();
//        }

//        public async Task AddAsync(Appointment appointment)
//        {
//            await _context.Appointments.AddAsync(appointment);
//        }

//        public void Update(Appointment appointment)
//        {
//            _context.Appointments.Update(appointment);
//        }

//        public void Delete(Appointment appointment)
//        {
//            appointment.IsDeleted = true;
//            appointment.DeletedAt = DateTime.Now;
//            _context.Appointments.Update(appointment);
//        }

//        public async Task<int> SaveChangesAsync()
//        {
//            return await _context.SaveChangesAsync();
//        }

//        // Pagination and Search
//        public async Task<(IEnumerable<Appointment> data, int totalCount)> GetPagedAsync(
//            string? search = null,
//            int page = 1,
//            int pageSize = 10,
//            string? status = null,
//            int? doctorId = null,
//            int? patientId = null,
//            DateTime? fromDate = null,
//            DateTime? toDate = null)
//        {
//            var query = _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Include(a => a.Availability)
//                .Where(a => !a.IsDeleted);

//            // Apply filters
//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(a =>
//                    a.Doctor.User.FullName.Contains(search) ||
//                    a.Patient.User.FullName.Contains(search) ||
//                    a.Symptoms.Contains(search) ||
//                    a.Diagnosis.Contains(search));
//            }

//            if (!string.IsNullOrEmpty(status))
//            {
//                query = query.Where(a => a.Status == status);
//            }

//            if (doctorId.HasValue)
//            {
//                query = query.Where(a => a.DoctorId == doctorId.Value);
//            }

//            if (patientId.HasValue)
//            {
//                query = query.Where(a => a.PatientId == patientId.Value);
//            }

//            if (fromDate.HasValue)
//            {
//                query = query.Where(a => a.AppointmentDate >= fromDate.Value);
//            }

//            if (toDate.HasValue)
//            {
//                query = query.Where(a => a.AppointmentDate <= toDate.Value);
//            }

//            var totalCount = await query.CountAsync();

//            var data = await query
//                .OrderByDescending(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            return (data, totalCount);
//        }

//        // Specific queries
//        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
//        {
//            return await _context.Appointments
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
//                .Where(a => a.PatientId == patientId && !a.IsDeleted)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Where(a => a.AppointmentDate.Date == date.Date && !a.IsDeleted)
//                .OrderBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(string status)
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Where(a => a.Status == status && !a.IsDeleted)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync()
//        {
//            var today = DateTime.Today;
//            return await GetAppointmentsByDateAsync(today);
//        }

//        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7)
//        {
//            var fromDate = DateTime.Today;
//            var toDate = DateTime.Today.AddDays(days);

//            return await _context.Appointments
//                .Include(a => a.Doctor).ThenInclude(d => d.User)
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .Where(a => a.AppointmentDate >= fromDate && a.AppointmentDate <= toDate && !a.IsDeleted)
//                .OrderBy(a => a.AppointmentDate)
//                .ThenBy(a => a.StartTime)
//                .ToListAsync();
//        }

//        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null)
//        {
//            var conflictingAppointments = await _context.Appointments
//                .Where(a => a.DoctorId == doctorId &&
//                           a.AppointmentDate.Date == date.Date &&
//                           !a.IsDeleted &&
//                           a.Status != "Cancelled" &&
//                           (excludeAppointmentId == null || a.Id != excludeAppointmentId) &&
//                           ((a.StartTime < endTime && a.EndTime > startTime)))
//                .CountAsync();

//            return conflictingAppointments == 0;
//        }

//        // Helper methods for dropdowns
//        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
//        {
//            return await _context.Doctors
//                .Include(d => d.User)
//                .Include(d => d.Specialty)
//                //.Where(d => d.User.IsActive && !d.IsDeleted)

//                .OrderBy(d => d.User.FullName)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Patient>> GetActivePatientsAsync()
//        {
//            return await _context.Patients
//                .Include(p => p.User)
//                //.Where(p => p.User.IsActive && !p.IsDeleted)

//                .OrderBy(p => p.User.FullName)
//                .ToListAsync();
//        }

//        //public async Task<IEnumerable<Availability>> GetDoctorAvailabilitiesAsync(int doctorId)
//        //{
//        //    return await _context.Availabilities

//        //         .Where(a => a.DoctorId == doctorId)
//        //        .OrderBy(a => a.DayOfWeek)
//        //        .ThenBy(a => a.StartTime)
//        //        .ToListAsync();
//        //}

//        public async Task<IEnumerable<Availability>> GetDoctorAvailabilitiesAsync(int doctorId)
//        {
//            return await _context.Availabilities
//                //.Where(a => a.DoctorId == doctorId && a.IsActive)
//                //.OrderBy(a => a.DayOfWeek)
//                .OrderBy(a => a.StartTime)
//                .ToListAsync();
//        }


//        // Statistics
//        public async Task<int> GetTotalAppointmentsCountAsync()
//        {
//            return await _context.Appointments
//                .Where(a => !a.IsDeleted)
//                .CountAsync();
//        }

//        public async Task<int> GetAppointmentsCountByStatusAsync(string status)
//        {
//            return await _context.Appointments
//                .Where(a => a.Status == status && !a.IsDeleted)
//                .CountAsync();
//        }

//        public async Task<decimal> GetTotalRevenueAsync()
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor)
//                .Where(a => a.Status == "Completed" && !a.IsDeleted)
//                .SumAsync(a => a.Doctor.ConsultationFee);
//        }



//        //Task List<string>> GetStatusOptions();
//        public async Task<List<string>> GetStatusOptions() {
//            return await Task.FromResult(new


//            List<string>{ "Pending","Completed","Cancelled"});

//        }
//        public async Task<bool> HasConflictingAppointmentAsync
//         (int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeId = null)
//        {
//            //return await _context.Appointments.Where(a=>a.DoctorId==doctorId&&a.AppointmentDate==date 
//            //&& excludeId==null||)
//            //&&(||a.Id!=excludeId startTime >=)&& ((startTime>=a.StartTime&&startTime<a.EndTime)
//            //    ||(endTime>a.StartTime&& endTime<=a.EndTime)||
//            //    (startTime <= a.StartTime && endTime >= a.EndTime))).AnyAsync();

//            return await _context.Appointments
//              .Where(a => a.DoctorId == doctorId
//                && a.AppointmentDate == date
//                && (excludeId == null || a.Id != excludeId)
//                && ((startTime >= a.StartTime && startTime < a.EndTime) ||
//                    (endTime > a.StartTime && endTime <= a.EndTime) ||
//                    (startTime <= a.StartTime && endTime >= a.EndTime)))
//                 .AnyAsync();


//        }
//    }
//}



















