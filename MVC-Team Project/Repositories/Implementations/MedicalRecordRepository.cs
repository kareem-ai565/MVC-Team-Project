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
        private IQueryable<MedicalRecord> BaseQuery() => _context.MedicalRecords
            .Include(r => r.Doctor).ThenInclude(d => d.User)
            .Include(r => r.Patient).ThenInclude(p => p.User)
            .Where(r => !r.IsDeleted);

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync() =>
            await BaseQuery().ToListAsync();

        public async Task<MedicalRecord?> GetByIdAsync(int id) =>
            await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId) =>
            await BaseQuery()
                    .Where(r => r.DoctorId == doctorId)
                    .OrderByDescending(r => r.RecordDate)
                    .ToListAsync();

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId, bool includeConfidential = false) =>
            await BaseQuery()
                   .Where(r => r.PatientId == patientId &&
                              (includeConfidential || !r.IsConfidential))
                   .OrderByDescending(r => r.RecordDate)
                   .ToListAsync();

        public async Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> SearchByDoctorPagedAsync(int doctorId, string? search, int page, int pageSize)
        {
            var query = BaseQuery().Where(r => r.DoctorId == doctorId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(r =>
                    r.Patient.User.FullName.ToLower().Contains(search) ||
                    r.Diagnosis.ToLower().Contains(search) ||
                    r.RecordType.ToLower().Contains(search));
            }

            var total = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.RecordDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, total);
        }

        public async Task AddAsync(MedicalRecord record) =>
            await _context.MedicalRecords.AddAsync(record);

        public void Update(MedicalRecord record) =>
            _context.MedicalRecords.Update(record);
        public void SoftDelete(MedicalRecord record, int deletedById)
        {
            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;
            record.DeletedBy = deletedById;
            _context.MedicalRecords.Update(record);
        }
        public void Delete(MedicalRecord entity) =>
            _context.MedicalRecords.Remove(entity);
        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
