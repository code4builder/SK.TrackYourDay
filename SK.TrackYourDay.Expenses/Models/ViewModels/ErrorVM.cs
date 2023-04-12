using Newtonsoft.Json;

namespace SK.TrackYourDay.Expenses.Models.ViewModels
{
    public class ErrorVM
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
