using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;

namespace MVC_Team_Project.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ClinicSystemContext context;
        List<Payment> payments;

        public PaymentController(ClinicSystemContext context)
        {
            this.context = context;
             payments= context.Payments.ToList();
        }

        public IActionResult Index()
        {
           

            return View("Index", payments);
        }

        public IActionResult AddPayment()
        {
           

            return View("addPayment" );
        }


        public IActionResult SubmitAddPayment(Payment payment)
        {
            if (ModelState.IsValid) {


                context.Payments.Add(payment);
                context.SaveChanges();

                return RedirectToAction("Index");



            }
           

            

            return View("addPayment" , payment);
        }




        public IActionResult PaymentDetails(int id)
        {

         Payment payment= context.Payments.FirstOrDefault(p=>p.Id == id);


            return View("PaymentDetails", payment);

        }

    }



}
