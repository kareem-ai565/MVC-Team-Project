using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories
{
    public class PatientsRepository : IPatientRepository
    {
        private readonly ClinicSystemContext _context;

        public PatientsRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<Patient?> GetPatientWithUserAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> GetPagedPatientsAsync(int page, int pageSize)
        {
            return await _context.Patients
                .Include(p => p.User)
                .OrderBy(p => p.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Patients.CountAsync();
        }

        public async Task AddAsync(Patient entity)
        {
            await _context.Patients.AddAsync(entity);
        }

        public void Update(Patient entity)
        {
            _context.Patients.Update(entity);
        }

        public void Delete(Patient entity)
        {
            _context.Patients.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Patient?> GetByUserIdAsync(int userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

    }
}
