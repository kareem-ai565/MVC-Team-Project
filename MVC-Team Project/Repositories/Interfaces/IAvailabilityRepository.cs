using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IAvailabilityRepository: IRepository<Availability>
    {
        Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Availability>> GetByDateAsync(DateTime date);
        Task<bool> IsSlotTakenAsync(int doctorId, DateTime date, TimeSpan startTime);

    }
}
