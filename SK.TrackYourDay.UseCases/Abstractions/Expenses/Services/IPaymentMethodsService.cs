using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.UseCases.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.TrackYourDay.UseCases.Abstractions.Expenses.Services
{
    public interface IPaymentMethodsService
    {
        Task<List<PaymentMethodDTO>> GetAllPaymentMethodsDTOAsync(string userId);
        Task<List<PaymentMethodDTO>> GetPaymentMethodsDTOByUserIdAsync(string userId);
        Task<PaymentMethod> GetPaymentMethodByIdAsync(int id);
        Task<PaymentMethodDTO> GetPaymentMethodDTOByIdAsync(int id);
        Task CreatePaymentMethodAsync(PaymentMethodDTO paymentMethodDTO, string userId);
        Task DeletePaymentMethodByIdAsync(int id);
        Task<PaymentMethod> UpdatePaymentMethodByIdAsync(int id, PaymentMethodDTO paymentMethodDTO);
        Task<List<PaymentMethodDTO>> GetFriendsPaymentMethodsAsync(string userId);
        PaymentMethodDTO ConvertPaymentMethodToDTO(PaymentMethod paymentMethod, string userId);

    }
}
