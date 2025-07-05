using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        public Task AddAsync(Availability entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Availability entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Availability>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Availability?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Availability entity)
        {
            throw new NotImplementedException();
        }
    }
}
