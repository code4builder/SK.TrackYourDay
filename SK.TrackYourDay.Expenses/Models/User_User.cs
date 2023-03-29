namespace SK.TrackYourDay.Expenses.Models
{
    public class User_User
    {
        public int Id { get; set; }

        // Navigation Properties
        public int User1Id { get; set; }
        public User User1 { get; set; }

        public int User2Id { get; set; }
        public User User2 { get; set; }
    }
}
