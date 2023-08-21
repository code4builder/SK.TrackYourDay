using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SK.TrackYourDay.Domain.Models;
using SK.TrackYourDay.Infrastructure.DataAccess;
using SK.TrackYourDay.UseCases.Abstractions.Expenses.Services;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.UseCases.Expenses.Services
{
    public class ExpensesService : IExpensesService
    {
        private ApplicationDbContext _context;
        private IMemoryCache _memoryCache;

        public ExpensesService(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
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
                expensesDTO = await GetExpensesDTOByUserIdAsync(userId);
                var friendsExpensesDTO = await GetFriendsExpensesAsync(userId);
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

            return expensesDTO;
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpensesDTOCache(string userId, string role, string sortBy,
                                                                    string searchString, int? pageNumber, int? pageSize)
        {
            List<ExpenseDTO> expensesDTO;

            return _memoryCache.Get<List<ExpenseDTO>>("expenses");
        }

        public void SetAllExpensesDTOToCache(string key, IEnumerable<ExpenseDTO> expensesDTO)
        {
            _memoryCache.Set(key, expensesDTO, TimeSpan.FromMinutes(1));
        }

        public async Task<List<ExpenseDTO>> GetExpensesDTOByUserIdAsync(string userId)
        {
            if (_context.Expenses.Any())
            {
                var expenses = await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
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
            var hasExpenseCategorySelected = await CheckExpenseCategorySelectedAsync(expenseDTO, userId);

            // Checking if payment method was selected
            var hasPaymentMethodSelected = await CheckPaymentMethodSelectedAsync(expenseDTO, userId);

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
                    UserId = userId,
                    IrregularPayment = expenseDTO.IrregularPayment
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

        public async Task<Expense> UpdateExpenseByIdAsync(int id, ExpenseDTO expenseDTO, string userId)
        {
            // Checking if category was selected
            var hasExpenseCategorySelected = await CheckExpenseCategorySelectedAsync(expenseDTO, userId);

            // Checking if payment method was selected
            var hasPaymentMethodSelected = await CheckPaymentMethodSelectedAsync(expenseDTO, userId);

            var _expense = await _context.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
            if (_expense != null)
            {
                _expense.ExpenseName = expenseDTO.ExpenseName;
                _expense.Description = expenseDTO.Description;
                _expense.Amount = expenseDTO.Amount;
                _expense.ExpenseCategory = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == int.Parse(expenseDTO.ExpenseCategory));
                _expense.PaymentMethod = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == int.Parse(expenseDTO.PaymentMethod));
                _expense.Date = expenseDTO.Date;
                _expense.IrregularPayment = expenseDTO.IrregularPayment;

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
            var expenseCat = _context.ExpenseCategories.FirstOrDefault(ec => ec.Id == expense.ExpenseCategoryId).Name.ToString();
            var paymentMet = _context.PaymentMethods.FirstOrDefault(pm => pm.Id == expense.PaymentMethodId).Name.ToString();

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
                    UserName = GetFullUserName(userId),
                    IrregularPayment = expense.IrregularPayment
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
        public async Task<bool> CheckExpenseCategorySelectedAsync(ExpenseDTO expenseDTO, string userId)
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
                    var expenseCategoryService = new ExpenseCategoriesService(_context, _memoryCache);
                    var otherExpenseCategory = new ExpenseCategoryDTO() { Name = "Other", User = userId };
                    await expenseCategoryService.CreateExpenseCategoryAsync(otherExpenseCategory, userId);
                    var category = await _context.ExpenseCategories.FirstOrDefaultAsync(x => x.Name.ToLower() == "other");
                    expenseDTO.ExpenseCategory = category.Id.ToString();
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
        public async Task<bool> CheckPaymentMethodSelectedAsync(ExpenseDTO expenseDTO, string userId)
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
                    var paymentMethodService = new PaymentMethodsService(_context, _memoryCache);
                    var otherPaymentMethod = new PaymentMethodDTO() { Name = "Other", User = userId };
                    await paymentMethodService.CreatePaymentMethodAsync(otherPaymentMethod, userId);
                    var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Name.ToLower() == "other");
                    expenseDTO.PaymentMethod = paymentMethod.Id.ToString();
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

        public async Task<List<ExpenseDTO>> GetFriendsExpensesAsync(string userId)
        {
            var friends = await GetFriendsListAsync(userId);

            var friendsExpensesDTO = new List<ExpenseDTO>();
            foreach (var friend in friends)
            {
                var expensesDTO = await GetExpensesDTOByUserIdAsync(friend.Id);
                friendsExpensesDTO.AddRange(expensesDTO);
            }
            return friendsExpensesDTO;
        }

        public async Task<List<ApplicationUser>> GetFriendsListAsync(string userId)
        {
            var friends = await _context.User_Relations.Include(x => x.User2).Where(x => x.User1Id == userId).Select(x => x.User2).ToListAsync();
            var friendsRightColumn = await _context.User_Relations.Include(x => x.User1).Where(x => x.User2Id == userId).Select(x => x.User1).ToListAsync();
            friends.AddRange(friendsRightColumn);

            return friends;
        }

        public async Task<IEnumerable<ExpenseDTO>> FilterExpensesAsync(string userId, string role, FilterDTO filterDTO)
        {
            string requestedPaymentMethodName = _context.PaymentMethods.FirstOrDefault(x => x.Id.ToString() == filterDTO.PaymentMethod)?.Name;
            string requestedExpenseCategoryName = _context.ExpenseCategories.FirstOrDefault(x => x.Id.ToString() == filterDTO.ExpenseCategory)?.Name;

            var expenses = await GetAllExpensesDTOAsync(userId, role, null, null, 0, 10);

            List<ExpenseDTO> filteredExpenses = new List<ExpenseDTO>();

            if (expenses != null)
                filteredExpenses = expenses.FilterByDateRange(filterDTO)
                                            .FilterByExpenseName(filterDTO)
                                            .FilterByDescription(filterDTO)
                                            .FilterByPaymentMethod(requestedPaymentMethodName)
                                            .FilterByExpenseCategory(requestedExpenseCategoryName)
                                            .FilterByAmountRange(filterDTO)
                                            .FilterByIrregularPayment(filterDTO)
                                            .FilterByRegularPayment(filterDTO).ToList();

            return filteredExpenses;
        }

        public decimal GetTotalAmount(IEnumerable<ExpenseDTO> expensesDTO) => expensesDTO?.Sum(x => x.Amount) ?? 0;
    }
}

