using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ClinicSystemContext _context;

        public AppointmentRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        private IQueryable<Appointment> BaseQuery() =>
            _context.Appointments
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .Include(a => a.Patient).ThenInclude(p => p.User)
                    .Where(a => !a.IsDeleted);

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await BaseQuery().ToListAsync();

        public async Task<Appointment?> GetByIdAsync(int id) =>
            await BaseQuery().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId) =>
            await BaseQuery().Where(a => a.DoctorId == doctorId)
                             .OrderByDescending(a => a.AppointmentDate)
                             .ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId) =>
            await BaseQuery().Where(a => a.PatientId == patientId)
                             .OrderByDescending(a => a.AppointmentDate)
                             .ToListAsync();

        public async Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int doctorId)
        {
            return await BaseQuery()
                         .Where(a => a.DoctorId == doctorId)
                         .Select(a => a.AppointmentDate)
                         .Distinct()
                         .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorOnDateAsync(int doctorId, DateTime date)
        {
            return await BaseQuery()
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment) =>
            await _context.Appointments.AddAsync(appointment);

        public void Update(Appointment appointment) =>
            _context.Appointments.Update(appointment);

        public void SoftDelete(Appointment appointment, int deletedById)
        {
            appointment.IsDeleted = true;
            appointment.DeletedAt = DateTime.UtcNow;
            appointment.DeletedBy = deletedById;
            _context.Appointments.Update(appointment);
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public void Delete(Appointment entity)
        {
            throw new NotImplementedException();
        }
    }
}
