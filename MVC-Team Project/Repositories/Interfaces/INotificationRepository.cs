using MVC_Team_Project.Models;

namespace MVC_Team_Project.Repositories.Interfaces
{
    public interface INotificationRepository
    {

        List<Notification> GetAllNotifications(int id);
        Notification EditNotification(int id);

        void save();


        void Add(Notification notification);



    }
}
