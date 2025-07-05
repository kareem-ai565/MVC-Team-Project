using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories
{
    public class DoctorsRepository : IDoctorsRepository
    {
        private readonly ClinicSystemContext _context;

        public DoctorsRepository(ClinicSystemContext context)
        {
            _context = context;
        }
        public async Task<List<ApplicationUser>> GetAvailableUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Doctor == null) // Only users not already assigned
                .ToListAsync();
        }

        public async Task<List<Specialty>> GetActiveSpecialtiesAsync()
        {
            return await _context.Specialties
                .Where(s => s.IsActive)
                .ToListAsync();
        }


        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors.FindAsync(id);
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Appointments).ThenInclude(a => a.Patient).ThenInclude(p => p.User)
                .Include(d => d.Availabilities)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<(IEnumerable<Doctor> data, int totalCount)> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Appointments).ThenInclude(a => a.Patient).ThenInclude(p => p.User)
                .Include(d => d.Availabilities)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d => d.User.FullName.Contains(search) || d.Specialty.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(d => d.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }


        public async Task AddAsync(Doctor entity)
        {
            await _context.Doctors.AddAsync(entity);
        }

        public void Update(Doctor entity)
        {
            _context.Doctors.Update(entity);
        }

        public void Delete(Doctor entity)
        {
            _context.Doctors.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Doctor?> GetByUserIdWithDetailsAsync(int userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Availabilities)
                .Include(d => d.Appointments).ThenInclude(a => a.Patient).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

    }
}
