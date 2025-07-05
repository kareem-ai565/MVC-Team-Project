using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
<<<<<<< HEAD
    public interface IAvailabilityRepository: IRepository<Availability>
    {
        Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Availability>> GetByDateAsync(DateTime date);
        Task<bool> IsSlotTakenAsync(int doctorId, DateTime date, TimeSpan startTime);

=======
    public interface IAvailabilityRepository : IRepository<Availability>
    {
        Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId);
>>>>>>> 792ebac25cff0b5c4e8142fb38a771d72712746f
    }
}
