using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.Expenses.Services;
using AutoMapper;
using System.Security.Claims;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class PaymentMethodsController : Controller
    {
        private PaymentMethodsService _paymentMethodsService;
        private readonly IMapper _mapper;
        public PaymentMethodsController(PaymentMethodsService paymentMethodsService, IMapper mapper)
        {
            _paymentMethodsService = paymentMethodsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var objList = await _paymentMethodsService.GetAllPaymentMethodsDTOAsync(_userId);
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
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

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
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var paymentMethod = _paymentMethodsService.GetPaymentMethodByIdAsync((int)id);
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
                _paymentMethodsService.DeletePaymentMethodByIdAsync((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var paymentMethod = _paymentMethodsService.GetPaymentMethodByIdAsync((int)id);
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
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                _paymentMethodsService.UpdatePaymentMethodByIdAsync(paymentMethod.Id, paymentMethod);
                return RedirectToAction("Index");
            }

            return View(paymentMethod);
        }
    }
}
