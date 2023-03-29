namespace SK.TrackYourDay.Expenses.Data.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _db;
        public AccountService(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
