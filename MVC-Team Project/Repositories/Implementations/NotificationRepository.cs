using Microsoft.EntityFrameworkCore;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Interfaces;

namespace MVC_Team_Project.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ClinicSystemContext clinicSystemContext;

        public NotificationRepository(ClinicSystemContext clinicSystemContext)
        {
            this.clinicSystemContext = clinicSystemContext;
        }

        public List<Notification> GetAllNotifications(int id)
        {
            return  clinicSystemContext.Notifications.Where(n => n.UserId == id).ToList();

        }
    }
}
