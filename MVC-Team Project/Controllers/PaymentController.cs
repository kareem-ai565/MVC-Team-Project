using Microsoft.AspNetCore.Mvc;
using MVC_Team_Project.Models;
using MVC_Team_Project.Repositories.Implementations;
using MVC_Team_Project.Repositories.Interfaces;
using System.Threading.Tasks;

namespace MVC_Team_Project.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IpaymentRepository paymentRepository;

        public PaymentController(IpaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> Index()
        {

            IEnumerable<Payment> paymentList = await paymentRepository.GetAllAsync();



            return View("Index", paymentList);
        }

        public IActionResult AddPayment()
        {
           

            return View("addPayment" );
        }


        public IActionResult SubmitAddPayment(Payment payment)
        {
            
            if (ModelState.IsValid) {


                paymentRepository.AddAsync(payment);
                paymentRepository.SaveChangesAsync();
                return RedirectToAction("Index");

            }
           

            return View("addPayment" , payment);
        }




        public async Task<IActionResult> PaymentDetails(int id)
        {

         Payment payment= await paymentRepository.GetByIdAsync(id);


            return View("PaymentDetails", payment);

        }

    }



}
