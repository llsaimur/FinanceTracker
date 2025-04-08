namespace FinanceTrackerUI.Models.Requests
{
    public class CreateAccountRequest
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
