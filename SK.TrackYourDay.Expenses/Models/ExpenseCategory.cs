using MessagePack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SK.TrackYourDay.Expenses.Models
{
    public class ExpenseCategory
    {
        public int Id { get; set; }

        [DisplayName("Expense Category")]
        [Required]
        public string Name { get; set; }

        public ExpenseCategory()
        {
            
        }
        //Groceries,
        //Bills,
        //Transport,
        //Car,
        //Restaurants,
        //Entertainment,
        //Clothing,
        //Gifts,
        //Health,
        //Travel,
        //Insurance,
        //Other
    }
}
