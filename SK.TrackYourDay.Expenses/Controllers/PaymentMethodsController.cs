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
        public PaymentMethodsController(PaymentMethodsService paymentMethodsService, IMapper mapper)
        {
            _paymentMethodsService = paymentMethodsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var paymentMethodsDTO = await _paymentMethodsService.GetAllPaymentMethodsDTOAsync(_userId);
            var paymentMethodsVM = _mapper.Map<IEnumerable<PaymentMethodVM>>(paymentMethodsDTO);
            return View(paymentMethodsVM);
        }

        //GET-Create - Creating View
        public IActionResult Create()
        {
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
                return RedirectToAction("Index");
            }

            return View(paymentMethodVM);
        }

        // GET-Delete - Creating View
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
                await _paymentMethodsService.DeletePaymentMethodByIdAsync((int)id);

            return RedirectToAction("Index");
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if(id != null)
                await _paymentMethodsService.DeletePaymentMethodByIdAsync((int)id);

            return RedirectToAction("Index");
        }

        // GET-Update - Creating View
        public async Task<IActionResult> Update(int? id)
        {
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
                return RedirectToAction("Index");
            }

            return View(paymentMethodVM);
        }
    }
}
