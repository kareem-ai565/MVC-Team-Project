﻿using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface ISpecialtyRepository : IRepository<Specialty>
    {
        Task<Specialty?> GetByNameAsync(string name);
        Task<IEnumerable<Specialty>> GetActiveSpecialtiesAsync();

        Task<(IEnumerable<Specialty> data, int totalCount)> GetPagedAsync(string? search, int page, int pageSize);

        Task<SpecialtyForShowVM?> GetSpecialtyWithDoctorsAsync(int id);

    }
}
