using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Implementations;
using MVC_Team_Project.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC_Team_Project.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }
        public IActionResult Index()
        {
            int id =int.Parse( User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

           List<Notification> notifications=  notificationRepository.GetAllNotifications(id);

            return View("Index", notifications);

           


        }
    }
}
