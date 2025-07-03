using global::MVC_Team_Project.Models;
using global::MVC_Team_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
        public class MedicalRecordRepository : IMedicalRecordRepository
        {
            private readonly ClinicSystemContext _context;

            public MedicalRecordRepository(ClinicSystemContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
                => await _context.MedicalRecords
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .Where(r => !r.IsDeleted)
                    .ToListAsync();

            public async Task<MedicalRecord?> GetByIdAsync(int id)
                => await _context.MedicalRecords
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
                => await _context.MedicalRecords
                    .Where(r => r.DoctorId == doctorId && !r.IsDeleted)
                    .ToListAsync();

            public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId, bool includeConfidential = false)
                => await _context.MedicalRecords
                    .Where(r => r.PatientId == patientId && !r.IsDeleted && (includeConfidential || !r.IsConfidential))
                    .ToListAsync();

            public async Task AddAsync(MedicalRecord record)
                => await _context.MedicalRecords.AddAsync(record);

            public void Update(MedicalRecord record)
                => _context.MedicalRecords.Update(record);

            public void SoftDelete(MedicalRecord record, int deletedById)
            {
                record.IsDeleted = true;
                record.DeletedAt = DateTime.UtcNow;
                record.DeletedBy = deletedById;
                _context.MedicalRecords.Update(record);
            }

            public async Task SaveChangesAsync()
                => await _context.SaveChangesAsync();
        }
}
