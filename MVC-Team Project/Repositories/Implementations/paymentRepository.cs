using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class paymentRepository : IpaymentRepository
    {
        private readonly ClinicSystemContext context;

        public paymentRepository(ClinicSystemContext context)
        {
            this.context = context;
        }


        public Task AddAsync(Payment entity)
        {
            context.Add(entity);
          return  SaveChangesAsync();

        }

      
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await context.Payments.ToListAsync();
        }


        public async Task<Payment?> GetByIdAsync(int id)
        {
            Payment payment =context.Payments.SingleOrDefault(p=>p.Id == id);
            return payment;

        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }



        public void Update(Payment entity)
        {
            throw new NotImplementedException();
        }


        public void Delete(Payment entity)
        {
            throw new NotImplementedException();
        }

    }
}
