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



        [HttpPost]
        public IActionResult EditStatus(int id)
        {
            Notification notification=notificationRepository.EditNotification(id);


            notification.IsRead= true;

            notificationRepository.save();


            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult UnreadCount()
        {
            int id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int unreadCount = notificationRepository.GetAllNotifications(id).Count(n => !n.IsRead);

            return Json(new { count = unreadCount });
        }


        //[HttpPost]

        //public IActionResult AddNotification(Notification notification)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //        notificationRepository.Add(notification);
        //        notificationRepository.save();
        //        return RedirectToAction("Index");
        //    //}


        //    //else
        //    //{

        //    //}
        //}




    }
}
