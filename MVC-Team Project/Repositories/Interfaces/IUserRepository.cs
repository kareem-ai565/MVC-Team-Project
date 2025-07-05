using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
        Task<(IEnumerable<ApplicationUser> Data, int TotalCount)> GetPagedAsync(string? search, int page, int pageSize);
        Task<List<ApplicationUser>> GetUsersWithoutPatientAsync();

    }
}
