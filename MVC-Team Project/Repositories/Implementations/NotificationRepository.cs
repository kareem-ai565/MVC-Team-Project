using Microsoft.EntityFrameworkCore.Diagnostics;
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
            return clinicSystemContext.Notifications.Where(i=>i.UserId==id).ToList();
        }



        public Notification EditNotification(int id)
        {
            Notification notification = clinicSystemContext.Notifications.SingleOrDefault(o => o.Id == id);

            return notification;
        }


        public void save()
        {
            clinicSystemContext.SaveChanges();
        }


    }
}
