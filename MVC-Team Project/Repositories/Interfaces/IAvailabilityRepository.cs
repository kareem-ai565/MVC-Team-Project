using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IAvailabilityRepository : IRepository<Availability>
    {
        Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId);
    }
}
