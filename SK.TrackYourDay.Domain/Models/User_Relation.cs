namespace SK.TrackYourDay.Domain.Models
{
    public class User_Relation
    {
        public string Id { get; set; }

        // Navigation Properties
        public string User1Id { get; set; }
        public virtual ApplicationUser User1 { get; set; }

        public string User2Id { get; set; }
        public virtual ApplicationUser User2 { get; set; }
    }
}
