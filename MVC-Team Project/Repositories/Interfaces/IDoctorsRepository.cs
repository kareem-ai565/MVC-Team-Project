using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IDoctorsRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByIdWithDetailsAsync(int id);
        Task<(IEnumerable<Doctor> data, int totalCount)> GetPagedAsync(string? search, int page, int pageSize);
        Task<List<ApplicationUser>> GetAvailableUsersAsync();
        Task<List<Specialty>> GetActiveSpecialtiesAsync();
        Task<Doctor?> GetByUserIdWithDetailsAsync(int userId);


    }
}
