using System.Diagnostics;

namespace SK.TrackYourDay.UseCases.DTOs;

[DebuggerDisplay("{Name}")]
public class ExpenseCategoryDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string User { get; set; }
}
