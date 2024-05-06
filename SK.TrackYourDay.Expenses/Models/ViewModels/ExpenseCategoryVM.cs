using System.ComponentModel;
using System.Diagnostics;

namespace SK.TrackYourDay.Expenses.Models.ViewModels;

[DebuggerDisplay("{Name}")]
public class ExpenseCategoryVM
{
    public int Id { get; set; }

    public string Name { get; set; }

    [DisplayName("Created by")]
    public string User { get; set; } = "Unknown";
}
