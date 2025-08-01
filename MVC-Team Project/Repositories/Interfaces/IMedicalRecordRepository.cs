﻿using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{

        public interface IMedicalRecordRepository:IRepository<MedicalRecord>
    {
            Task<IEnumerable<MedicalRecord>> GetAllAsync();
            Task<MedicalRecord?> GetByIdAsync(int id);
            Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId);
        Task<(IEnumerable<MedicalRecord> Records, int TotalCount)> SearchByDoctorPagedAsync(int doctorId, string? search, int page, int pageSize);

        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId, bool includeConfidential = false);
            Task AddAsync(MedicalRecord record);
            void Update(MedicalRecord record);
            void SoftDelete(MedicalRecord record, int deletedById);
            Task SaveChangesAsync();
        }
}

