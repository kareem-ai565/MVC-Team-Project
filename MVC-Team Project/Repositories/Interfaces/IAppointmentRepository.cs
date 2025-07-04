using System.Collections.Generic;
using System.Threading.Tasks;
using global::MVC_Team_Project.Models;
using MVC_Team_Project.Models;
namespace MVC_Team_Project.Repositories.Interfaces
{
        public interface IAppointmentRepository:IRepository<Appointment>
        {
            Task<IEnumerable<Appointment>> GetAllAsync();
            Task<Appointment?> GetByIdAsync(int id);
            Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
            Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
            Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int doctorId);
            Task<IEnumerable<Appointment>> GetByDoctorOnDateAsync(int doctorId, DateTime date);
            Task AddAsync(Appointment appointment);
            void Update(Appointment appointment);
            void SoftDelete(Appointment appointment, int deletedById);
            Task SaveChangesAsync();
        }
 }
