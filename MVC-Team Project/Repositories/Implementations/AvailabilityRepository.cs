using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class AvailabilityRepository: IAvailabilityRepository
    {
        private readonly ClinicSystemContext _context;

        public AvailabilityRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Availability>> GetAllAsync() =>
            await _context.Availabilities.Include(a => a.Doctor).ToListAsync();

        public async Task<Availability?> GetByIdAsync(int id) =>
            await _context.Availabilities.FindAsync(id);

        public async Task AddAsync(Availability entity) =>
            await _context.Availabilities.AddAsync(entity);

        public void Update(Availability entity) =>
            _context.Availabilities.Update(entity);

        public void Delete(Availability entity) =>
            _context.Availabilities.Remove(entity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId) =>
            await _context.Availabilities
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

        public async Task<IEnumerable<Availability>> GetByDateAsync(DateTime date) =>
            await _context.Availabilities
                .Where(a => a.AvailableDate == date)
                .ToListAsync();

        public async Task<bool> IsSlotTakenAsync(int doctorId, DateTime date, TimeSpan startTime) =>
            await _context.Availabilities.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AvailableDate == date &&
                a.StartTime == startTime);
    }
}

