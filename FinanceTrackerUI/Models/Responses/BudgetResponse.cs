namespace FinanceTrackerUI.Models.Responses
{
    public class BudgetResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
