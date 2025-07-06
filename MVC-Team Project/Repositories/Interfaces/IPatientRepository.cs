using global::MVC_Team_Project.Models;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;


    namespace MVC_Team_Project.Repositories.Interfaces
    {
        public interface IPatientRepository : IRepository<Patient>
        {
            Task<Patient?> GetPatientWithUserAsync(int id);
            Task<IEnumerable<Patient>> GetPagedPatientsAsync(int page, int pageSize);
            Task<int> GetTotalCountAsync();
            Task<Patient?> GetByUserIdAsync(int userId);
        Task<PatientVM?> GetPatientVMByIdAsync(int id);



    }
}



