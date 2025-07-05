using MVC_Team_Project.Models;
namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface IPatientRepository
    {

        Task<List<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(int id);

    }
}
