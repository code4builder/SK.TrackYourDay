using SK.TrackYourDay.Expenses.Models;

namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class PaymentMethodsService
    {
        private ApplicationDbContext _context;

        public PaymentMethodsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PaymentMethod> GetAllPaymentMethods() => _context.PaymentMethods.ToList();

        public PaymentMethod GetPaymentMethodById(int id) => _context.PaymentMethods.FirstOrDefault(x => x.Id == id);
        
        public void CreatePaymentMethod(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Add(paymentMethod);
            _context.SaveChanges();
        }

        public void DeletePaymentMethodById(int id)
        {
            var _paymentMethod = GetPaymentMethodById(id);
            if (_paymentMethod != null)
            {
                _context.PaymentMethods.Remove(_paymentMethod);
                _context.SaveChanges();
            }
        }

        public PaymentMethod UpdatePaymentMethodById(int id, PaymentMethod paymentMethod)
        {
            var _paymentMethod = GetPaymentMethodById(id);
            if (paymentMethod != null)
                _paymentMethod.Name = paymentMethod.Name;

            _context.SaveChanges();

            return _paymentMethod;
        }
    }
}
