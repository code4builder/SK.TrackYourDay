using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class PaymentMethodsService
    {
        private ApplicationDbContext _context;

        public PaymentMethodsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethodDTO>> GetAllPaymentMethodsDTOAsync(string userId) 
        {
            if (_context.PaymentMethods.Any())
            {
                var paymentMethods = await GetPaymentMethodsDTOByUserId(userId);
                var friendsPaymentMethods = await GetFriendsPaymentMethods(userId);
                paymentMethods.AddRange(friendsPaymentMethods);

                return paymentMethods.OrderBy(ec => ec.Name).ToList();
            }
            else
                return new List<PaymentMethodDTO>();
        }

        public async Task<List<PaymentMethodDTO>> GetPaymentMethodsDTOByUserId(string userId)
        {
            if (_context.PaymentMethods.Any())
            {
                var paymentMethods = await _context.PaymentMethods.Where(e => e.UserId == userId).ToListAsync();

                var paymentMethodsDTO = new List<PaymentMethodDTO>();
                foreach (var paymentMethod in paymentMethods)
                {
                    var paymentMethodDTO = ConvertPaymentMethodToDTO(paymentMethod, userId);
                    paymentMethodsDTO.Add(paymentMethodDTO);
                }
                return paymentMethodsDTO;
            }
            else
                return new List<PaymentMethodDTO>();
        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(int id) => await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<PaymentMethodDTO> GetPaymentMethodDTOByIdAsync(int id)
        {
            var paymentMethod = await GetPaymentMethodByIdAsync(id);
            var paymentMethodDTO = ConvertPaymentMethodToDTO(paymentMethod, paymentMethod.UserId);
            return paymentMethodDTO;
        }

        public async Task CreatePaymentMethodAsync(PaymentMethodDTO paymentMethodDTO, string userId)
        {
            var paymentMethod = new PaymentMethod()
            {
                Name = paymentMethodDTO.Name,
                UserId = userId
            };

            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentMethodByIdAsync(int id)
        {
            var _paymentMethod = await GetPaymentMethodByIdAsync(id);
            if (_paymentMethod != null)
            {
                _context.PaymentMethods.Remove(_paymentMethod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaymentMethod> UpdatePaymentMethodByIdAsync(int id, PaymentMethodDTO paymentMethodDTO)
        {
            var _paymentMethod = await GetPaymentMethodByIdAsync(id);
            if (paymentMethodDTO != null)
                _paymentMethod.Name = paymentMethodDTO.Name;

            await _context.SaveChangesAsync();

            return _paymentMethod;
        }

        public async Task<List<PaymentMethodDTO>> GetFriendsPaymentMethods(string userId)
        {
            var expenseService = new ExpensesService(_context);
            var friends = expenseService.GetFriendsList(userId);

            var friendsPaymentMethodsDTO = new List<PaymentMethodDTO>();
            foreach (var friend in friends)
            {
                var paymentMethodsDTO = await GetPaymentMethodsDTOByUserId(friend.Id);
                friendsPaymentMethodsDTO.AddRange(paymentMethodsDTO);
            }
            return friendsPaymentMethodsDTO;
        }

        public PaymentMethodDTO ConvertPaymentMethodToDTO(PaymentMethod paymentMethod, string userId)
        {
            var expenseService = new ExpensesService(_context);
            try
            {
                var paymentMethodDTO = new PaymentMethodDTO()
                {
                    Id = paymentMethod.Id,
                    Name = paymentMethod.Name,
                    User = expenseService.GetFullUserName(userId)
                };
                return paymentMethodDTO;
            }
            catch (Exception)
            {
                throw new Exception("Can not be converted");
            }
        }
    }
}
