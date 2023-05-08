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

        public ExpensesService(ApplicationDbContext context
         )
        {
            _context = context;
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
            List<ExpenseDTO> expensesDTO;

            if (role == RoleDTO.User)
            {
                expensesDTO = await GetExpensesDTOByUserId(userId);
                var friendsExpensesDTO = await GetFriendsExpenses(userId);
                expensesDTO.AddRange(friendsExpensesDTO);
            }
            else
            {
                var allExpenses = await _context.Expenses.ToListAsync();
                expensesDTO = ConvertListExpensesToDTO(allExpenses);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                expensesDTO = expensesDTO.Where(p => p.ExpenseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        expensesDTO = expensesDTO.OrderByDescending(p => p.ExpenseName).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
                expensesDTO = expensesDTO.OrderByDescending(e => e.Date).ThenBy(e => e.ExpenseName).ToList();

            // Paging
            int? pageSizeCorr = pageSize == 0 ? 10 : pageSize;
            expensesDTO = PaginatedList<ExpenseDTO>.Create(expensesDTO.AsQueryable(), pageNumber ?? 1, pageSizeCorr ?? 10);

            return expensesDTO;
        }

        public async Task<List<ExpenseDTO>> GetExpensesDTOByUserId(string userId)
        {
            if (_context.Expenses.Any())
            {
                var expenses = _context.Expenses.Where(e => e.UserId == userId);
                var expensesDTO = new List<ExpenseDTO>();
                foreach (var expense in expenses)
                {
                    var expenseDTO = ConvertExpenseToDTO(expense, userId);
                    expensesDTO.Add(expenseDTO);
                }
                return expensesDTO;
            }
            else
                return new List<ExpenseDTO>();
        }

        public async Task<ExpenseDTO> GetExpenseDTOByIdAsync(int expenseId, string userId)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == expenseId);
            var expenseDTO = ConvertExpenseToDTO(expense, userId);
            return expenseDTO;
        }

        public async Task AddExpenseAsync(ExpenseDTO expenseDTO, string userId)
        {
            // Checking if category was selected
            var hasExpenseCategorySelected = await CheckExpenseCategorySelected(expenseDTO, userId);

            // Checking if payment method was selected
            var hasPaymentMethodSelected = await CheckPaymentMethodSelected(expenseDTO, userId);

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
            await _context.SaveChangesAsync();
        }

        public async Task<Expense> UpdateExpenseById(int id, ExpenseDTO expenseDTO, string userId)
        {
            // Checking if category was selected
            var hasExpenseCategorySelected = await CheckExpenseCategorySelected(expenseDTO, userId);

            // Checking if payment method was selected
            var hasPaymentMethodSelected = await CheckPaymentMethodSelected(expenseDTO, userId);

            var _expense = await _context.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
            if (_expense != null)
            {
                _expense.ExpenseName = expenseDTO.ExpenseName;
                _expense.Description = expenseDTO.Description;
                _expense.Amount = expenseDTO.Amount;
                _expense.ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == int.Parse(expenseDTO.ExpenseCategory));
                _expense.PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == int.Parse(expenseDTO.PaymentMethod));
                _expense.Date = expenseDTO.Date;

                await _context.SaveChangesAsync();
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
                    await _context.SaveChangesAsync();
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

        public List<ExpenseDTO> ConvertListExpensesToDTO(List<Expense> expenses)
        {
            if (expenses.Any())
            {
                var expensesDTO = new List<ExpenseDTO>();
                foreach (var expense in expenses)
                {
                    var expenseDTO = ConvertExpenseToDTO(expense, expense.UserId);
                    expensesDTO.Add(expenseDTO);
                }
                return expensesDTO;
            }
            else
                return new List<ExpenseDTO>();
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

        /// <summary>
        /// Checking if expense category was not selected in the form correctly
        /// </summary>
        /// <param name="expenseDTO">Expense</param>
        public async Task<bool> CheckExpenseCategorySelected(ExpenseDTO expenseDTO, string userId)
        {
            if (!int.TryParse(expenseDTO.ExpenseCategory, out int result))
            {
                var hasOtherCategory = _context.ExpenseCategories.Any(x => x.Name.ToLower() == "other");
                if (hasOtherCategory)
                {
                    var category = await _context.ExpenseCategories.FirstOrDefaultAsync(x => x.Name.ToLower() == "other");
                    expenseDTO.ExpenseCategory = category.Id.ToString();
                }
                else
                {
                    var expenseCategoryService = new ExpenseCategoriesService(_context);
                    var otherExpenseCategory = new ExpenseCategoryDTO() { Name = "Other", User = userId };
                    await expenseCategoryService.CreateExpenseCategory(otherExpenseCategory, userId);
                }

                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Checking if payment method was not selected in the form correctly
        /// </summary>
        /// <param name="expenseDTO">Expense</param>
        public async Task<bool> CheckPaymentMethodSelected(ExpenseDTO expenseDTO, string userId)
        {
            if (!int.TryParse(expenseDTO.PaymentMethod, out int result))
            {
                var hasOtherPayment = _context.PaymentMethods.Any(x => x.Name.ToLower() == "other");
                if (hasOtherPayment)
                {
                    var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Name.ToLower() == "other");
                    expenseDTO.PaymentMethod = paymentMethod.Id.ToString();
                }
                else
                {
                    await _context.PaymentMethods.AddAsync(new PaymentMethod() { Name = "Other" });
                    await _context.SaveChangesAsync();
                }

                return false;
            }

            else 
                return true;
        }

        public async Task AddFriendAsync(string currentUserId, string friendEmail)
        {
            var doesUserExist = await _context.Users.AnyAsync(x => x.Email == friendEmail);
            if (doesUserExist)
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
                var friendUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == friendEmail);

                var userRelation = new User_Relation()
                {
                    User1Id = currentUser.Id,
                    User2Id = friendUser.Id
                };
                await _context.User_Relations.AddAsync(userRelation);

                currentUser.HasUserRelations = true;
                friendUser.HasUserRelations = true;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ExpenseDTO>> GetFriendsExpenses(string userId)
        {
            var friends = GetFriendsList(userId);

            var friendsExpensesDTO = new List<ExpenseDTO>();
            foreach (var friend in friends)
            {
                var expensesDTO = await GetExpensesDTOByUserId(friend.Id);
                friendsExpensesDTO.AddRange(expensesDTO);
            }
            return friendsExpensesDTO;
        }

        public List<ApplicationUser> GetFriendsList(string userId)
        {
            var friends = _context.User_Relations.Include(x => x.User2).Where(x => x.User1Id == userId).Select(x => x.User2).ToList();
            var friendsRightColumn = _context.User_Relations.Include(x => x.User1).Where(x => x.User2Id == userId).Select(x => x.User1).ToList();
            friends.AddRange(friendsRightColumn);

            return friends;
        }
    }
}

