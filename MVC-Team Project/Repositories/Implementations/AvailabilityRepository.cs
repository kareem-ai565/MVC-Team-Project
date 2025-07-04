using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly ClinicSystemContext _context;

        public AvailabilityRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Availability>> GetAllAsync()
        {
            return await _context.Availabilities
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AvailableDate)
                .ToListAsync();
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            return await _context.Availabilities
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Availabilities
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AvailableDate)
                .ToListAsync();
        }

        public async Task AddAsync(Availability availability)
        {
            await _context.Availabilities.AddAsync(availability);
        }

        public void Update(Availability availability)
        {
            _context.Availabilities.Update(availability);
        }

        public void Delete(Availability availability)
        {
            _context.Availabilities.Remove(availability);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
