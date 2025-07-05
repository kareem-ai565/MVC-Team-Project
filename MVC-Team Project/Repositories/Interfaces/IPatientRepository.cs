using global::MVC_Team_Project.Models;
using MVC_Team_Project.Models;


    namespace MVC_Team_Project.Repositories.Interfaces
    {
        public interface IPatientRepository : IRepository<Patient>
        {
            Task<Patient?> GetPatientWithUserAsync(int id);
            Task<IEnumerable<Patient>> GetPagedPatientsAsync(int page, int pageSize);
            Task<int> GetTotalCountAsync();

        }
    }



