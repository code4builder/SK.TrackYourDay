using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.Expenses.Services;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class PaymentMethodsController : Controller
    {
        private PaymentMethodsService _paymentMethodsService;
        public PaymentMethodsController(PaymentMethodsService paymentMethodsService)
        {
            _paymentMethodsService = paymentMethodsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var objList = _paymentMethodsService.GetAllPaymentMethods();
            return View(objList);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                _paymentMethodsService.CreatePaymentMethod(paymentMethod);
                return RedirectToAction("Index");
            }

            return View(paymentMethod);
        }

        // GET-Delete - Creating View
        public IActionResult Delete(int? id)
        {
            try
            {
                var paymentMethod = _paymentMethodsService.GetPaymentMethodById((int)id);
                return View(paymentMethod);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if(id != null)
                _paymentMethodsService.DeletePaymentMethodById((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var paymentMethod = _paymentMethodsService.GetPaymentMethodById((int)id);
            if (paymentMethod == null)
            {
                return NotFound();
            }
            return View(paymentMethod);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                _paymentMethodsService.UpdatePaymentMethodById(paymentMethod.Id, paymentMethod);
                return RedirectToAction("Index");
            }

            return View(paymentMethod);
        }
    }
}
