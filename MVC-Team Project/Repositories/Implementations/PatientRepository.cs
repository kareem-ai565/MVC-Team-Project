using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
namespace MVC_Team_Project.Repositories.Implementations
{
  
    public class PatientRepository : IPatientRepository
    {
        private readonly ClinicSystemContext _context;

        public PatientRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.Include(p => p.User).ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        }
    }

}
