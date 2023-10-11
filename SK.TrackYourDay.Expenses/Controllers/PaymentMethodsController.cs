using Microsoft.AspNetCore.Mvc;
using SK.TrackYourDay.Expenses.Data;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.Expenses.Services;
using AutoMapper;
using System.Security.Claims;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Controllers
{
    public class PaymentMethodsController : Controller
    {
        private PaymentMethodsService _paymentMethodsService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public PaymentMethodsController(PaymentMethodsService paymentMethodsService, IMapper mapper, ILogger<AccountController> logger)
        {
            _paymentMethodsService = paymentMethodsService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("GetAllPaymentMethods triggered");

            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var paymentMethodsDTO = await _paymentMethodsService.GetAllPaymentMethodsDTOAsync(_userId);
            var paymentMethodsVM = _mapper.Map<IEnumerable<PaymentMethodVM>>(paymentMethodsDTO);
            return View(paymentMethodsVM);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
            _logger.LogInformation("CreatePaymentMethod triggered");

            return View();
        }

        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentMethodVM paymentMethodVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var paymentMethodDTO = _mapper.Map<PaymentMethodDTO>(paymentMethodVM);
                await _paymentMethodsService.CreatePaymentMethodAsync(paymentMethodDTO, _userId);
                _logger.LogInformation($"The new payment method {paymentMethodVM.Name} was created");
                TempData["success"] = "Payment method created successfully";

                return RedirectToAction("Index");
            }

            return View(paymentMethodVM);
        }

        // GET-Delete - Creating View
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("DeletePaymentMethod triggered");

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var paymentMethodDTO = await _paymentMethodsService.GetPaymentMethodDTOByIdAsync((int)id);
            if (paymentMethodDTO == null)
            {
                return NotFound();
            }

            var paymentMethodVM = _mapper.Map<PaymentMethodVM>(paymentMethodDTO);
            if (paymentMethodVM == null)
            {
                return NotFound();
            }
            return View(paymentMethodVM);
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
            {
                await _paymentMethodsService.DeletePaymentMethodByIdAsync((int)id);
                _logger.LogInformation($"The payment method with {id} was deleted");
                TempData["success"] = "Payment method deleted successfully";
            }

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public async Task<IActionResult> Update(int? id)
        {
            _logger.LogInformation("UpdatePaymentMethod triggered");

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var paymentMethodDTO = await _paymentMethodsService.GetPaymentMethodDTOByIdAsync((int)id);
            if (paymentMethodDTO == null)
            {
                return NotFound();
            }

            var paymentMethodVM = _mapper.Map<PaymentMethodVM>(paymentMethodDTO);
            if (paymentMethodVM == null)
            {
                return NotFound();
            }
            return View(paymentMethodVM);
        }

        //POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PaymentMethodVM paymentMethodVM)
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var paymentMethodDTO = _mapper.Map<PaymentMethodDTO>(paymentMethodVM);
                await _paymentMethodsService.UpdatePaymentMethodByIdAsync(paymentMethodDTO.Id, paymentMethodDTO);
                _logger.LogInformation($"The payment method with {paymentMethodVM.Name} was updated");
                TempData["success"] = "Payment method updated successfully";

                return RedirectToAction("Index");
            }

            return View(paymentMethodVM);
        }
    }
}
