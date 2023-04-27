using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.DTOs;
using SK.TrackYourDay.UseCases.Expenses.Paging;
using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class ExpensesService
    {
        private ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public ExpensesService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all expenses as View Models
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="role">Role</param>
        /// <param name="sortBy"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Returns all expenses DTO</returns>
        public async Task<IEnumerable<ExpenseDTO>> GetAllExpensesDTOAsync(string userId, string role, string sortBy,
                                                                    string searchString, int? pageNumber, int? pageSize)
        {
            List<Expense> expenses;

            if (role == RoleDTO.User)
            {
                expenses = GetExpensesByUserId(userId).OrderByDescending(e => e.Date).ToList();
                expenses = expenses.ToList();
            }
            else
                expenses = _context.Expenses.OrderByDescending(e => e.Date).ToList();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        expenses = expenses.OrderByDescending(p => p.ExpenseName).ToList();
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(p => p.ExpenseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            // Paging
            int? pageSizeCorr = pageSize == 0 ? 10 : pageSize;
            expenses = PaginatedList<Expense>.Create(expenses.AsQueryable(), pageNumber ?? 1, pageSizeCorr ?? 10);

            var expensesDTO = new List<ExpenseDTO>();

            if (expenses.Any())
            {
                foreach (var expense in expenses)
                {
                    var expenseDTO = ConvertExpenseToDTO(expense, expense.UserId);

                    expensesDTO.Add(expenseDTO);
                }
            }

            return expensesDTO;
        }

        public List<Expense> GetExpensesByUserId(string userId)
        {
            if (_context.Expenses.Any())
                return _context.Expenses.Where(e => e.UserId == userId).ToList();
            else
                return new List<Expense>();
        }

        public async Task<ExpenseDTO> GetExpenseDTOByIdAsync(int expenseId, string userId)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == expenseId);
            var expenseDTO = ConvertExpenseToDTO(expense, userId);
            return expenseDTO;
        }


        public async Task AddExpenseAsync(ExpenseDTO expenseDTO, string userId)
        {
            // Checking if category and payment method was selected
            CheckIfExpenseCategoryNotSelected(expenseDTO);
            CheckIfPaymentMethodNotSelected(expenseDTO);

            Expense expense;
            try
            {
                expense = new Expense()
                {
                    Id = expenseDTO.Id,
                    ExpenseName = expenseDTO.ExpenseName,
                    Description = expenseDTO.Description,
                    Amount = expenseDTO.Amount,
                    ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == int.Parse(expenseDTO.ExpenseCategory)),
                    PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == int.Parse(expenseDTO.PaymentMethod)),
                    Date = expenseDTO.Date,
                    UserId = userId
                };
            }
            catch (Exception)
            {
                throw new Exception("This expense can not be created");
            }

            if (expense.ExpenseCategory == null || expense.PaymentMethod == null)
                throw new Exception("This expense can not be created");

            await _context.Expenses.AddAsync(expense);
            _context.SaveChanges();
        }

        public async Task<Expense> UpdateExpenseById(int id, ExpenseDTO expenseDTO)
        {
            // Checking if category and payment method was selected
            CheckIfExpenseCategoryNotSelected(expenseDTO);
            CheckIfPaymentMethodNotSelected(expenseDTO);

            var _expense = await _context.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
            if (_expense != null)
            {
                _expense.ExpenseName = expenseDTO.ExpenseName;
                _expense.Description = expenseDTO.Description;
                _expense.Amount = expenseDTO.Amount;
                _expense.ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == int.Parse(expenseDTO.ExpenseCategory));
                _expense.PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == int.Parse(expenseDTO.PaymentMethod));
                _expense.Date = expenseDTO.Date;

                _context.SaveChanges();
            }
            return _expense;
        }

        public async Task DeleteExpenseByIdAsync(int id)
        {
            try
            {
                var _expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id);
                if (_expense != null)
                {
                    _context.Expenses.Remove(_expense);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw new Exception("Can not be deleted");
            }
        }

        public ExpenseDTO ConvertExpenseToDTO(Expense expense, string userId)
        {
            try
            {
                var expenseDTO = new ExpenseDTO()
                {
                    Id = expense.Id,
                    ExpenseName = expense.ExpenseName,
                    Description = expense.Description,
                    Amount = expense.Amount,
                    ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == expense.ExpenseCategoryId).Name.ToString(),
                    PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == expense.PaymentMethodId).Name.ToString(),
                    Date = expense.Date,
                    UserName = GetFullUserName(userId)
                };
                return expenseDTO;
            }
            catch (Exception)
            {
                throw new Exception("Can not be converted");
            }
        }

        public string GetFullUserName(string userId)
        {
            var _user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return _user.FirstName + " " + _user.LastName;
        }

        public Expense GetExpenseById(int id) => _context.Expenses.FirstOrDefault(x => x.Id == id);

        public List<Expense> GetExpensesByDate(DateTime date) => _context.Expenses.Where(x => x.Date == date).ToList();

        public List<Expense> GetExpensesByMonth(DateTime date) => _context.Expenses.Where(x => x.Date.Month == date.Month).ToList();

        public List<Expense> GetExpensesByYear(DateTime date) => _context.Expenses.Where(x => x.Date.Year == date.Year).ToList();

        public List<Expense> GetExpensesByUserId(int userId) => _context.Expenses.Where(x => x.UserId.ToString() == userId.ToString()).ToList();

        /// <summary>
        /// Checking if expense category was not selected in the form correctly
        /// </summary>
        /// <param name="expenseDTO">Expense</param>
        public void CheckIfExpenseCategoryNotSelected(ExpenseDTO expenseDTO)
        {
            if (!int.TryParse(expenseDTO.ExpenseCategory, out int result))
            {
                var hasOtherCategory = _context.ExpenseCategories.Any(x => x.Name.ToLower() == "other");
                if (hasOtherCategory)
                {
                    var category = _context.ExpenseCategories.FirstOrDefault(x => x.Name.ToLower() == "other");
                    expenseDTO.ExpenseCategory = category.Id.ToString();
                }
                else
                {
                    _context.ExpenseCategories.Add(new ExpenseCategory() { Name = "Other" });
                    _context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Checking if payment method was not selected in the form correctly
        /// </summary>
        /// <param name="expenseDTO">Expense</param>
        public void CheckIfPaymentMethodNotSelected(ExpenseDTO expenseDTO)
        {
            if (!int.TryParse(expenseDTO.PaymentMethod, out int result))
            {
                var hasOtherPayment = _context.PaymentMethods.Any(x => x.Name.ToLower() == "other");
                if (hasOtherPayment)
                {
                    var paymentMethod = _context.PaymentMethods.FirstOrDefault(x => x.Name.ToLower() == "other");
                    expenseDTO.PaymentMethod = paymentMethod.Id.ToString();
                }
                else
                {
                    _context.PaymentMethods.Add(new PaymentMethod() { Name = "Other" });
                    _context.SaveChanges();
                }
            }
        }
    }
}

