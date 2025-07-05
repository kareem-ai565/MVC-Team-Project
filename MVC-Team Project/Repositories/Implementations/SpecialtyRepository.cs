using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly ClinicSystemContext _context;

        public SpecialtyRepository(ClinicSystemContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            return await _context.Specialties.ToListAsync();
        }

        public async Task<Specialty?> GetByIdAsync(int id)
        {
            //return await _context.Specialties.FindAsync(id);
            return await _context.Specialties
        .Include(s => s.Doctors)
        .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Specialty?> GetByNameAsync(string name)
        {
            return await _context.Specialties
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<Specialty>> GetActiveSpecialtiesAsync()
        {
            return await _context.Specialties
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(Specialty entity)
        {
            await _context.Specialties.AddAsync(entity);
        }

        public void Update(Specialty entity)
        {
            _context.Specialties.Update(entity);
        }

        public void Delete(Specialty entity)
        {
            _context.Specialties.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<(IEnumerable<Specialty> data, int totalCount)> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = _context.Specialties.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }


        // SpecialtyRepository.cs

        public async Task<SpecialtyForShowVM?> GetSpecialtyWithDoctorsAsync(int id)
        {
            var specialty = await _context.Specialties
                .Include(s => s.Doctors)
                    .ThenInclude(d => d.User) // ensure Doctor.User is included
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialty == null) return null;

            return new SpecialtyForShowVM
            {
                Id = specialty.Id,
                Name = specialty.Name,
                Description = specialty.Description,
                IsActive = specialty.IsActive,
                CreatedAt = specialty.CreatedAt,
                Doctors = specialty.Doctors.Select(d => new DoctorsVM
                {
                    Id = d.Id,
                    FullName = d.User.FullName,
                    SpecialtyName = specialty.Name,
                    Bio = d.Bio,
                    ClinicAddress = d.ClinicAddress,
                    ConsultationFee = d.ConsultationFee,
                    ExperienceYears = d.ExperienceYears,
                    IsVerified = d.IsVerified
                }).ToList()
            };
        }


    }
}
