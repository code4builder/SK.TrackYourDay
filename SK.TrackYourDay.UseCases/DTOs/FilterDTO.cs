namespace SK.TrackYourDay.UseCases.DTOs
{
    public class FilterDTO
    {
        public string? ExpenseName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? ExpenseCategory { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; } = DateTime.Now;
        public DateTime? DateTo { get; set; } = DateTime.Now;
        public bool IrregularPayment { get; set; } = true;
        public bool RegularPayment { get; set; } = true;
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
    }
}
