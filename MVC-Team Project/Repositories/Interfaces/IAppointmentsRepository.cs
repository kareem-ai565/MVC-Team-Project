using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;
using System;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IAppointmentsRepository
    {  
        Task<List<AppointmentVM>> GetAppointmentsAsync(string search, string status, DateTime? date, int page, int pageSize);
        Task<AppointmentVM> GetAppointmentByIdAsync(int id);
        Task<AppointmentEditVM> GetAppointmentForEditAsync(int id);
        Task<(bool Success, string Message)> CreateAppointmentAsync(AppointmentCreateVM model);
        Task<(bool Success, string Message)> UpdateAppointmentAsync(AppointmentEditVM model);
        Task<(bool Success, string Message)> CancelAppointmentAsync(int id, string reason);
        Task<(bool Success, string Message)> CompleteAppointmentAsync(AppointmentCompleteVM model);
        Task<bool> CheckAppointmentConflictAsync(int doctorId, DateTime date, TimeSpan start, TimeSpan end);
        Task<List<AppointmentVM>> GetAppointmentsByDateAsync(DateTime date);
        Task<List<AppointmentVM>> GetDoctorScheduleAsync(int? doctorId, DateTime date);
        Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
    
    }
}



