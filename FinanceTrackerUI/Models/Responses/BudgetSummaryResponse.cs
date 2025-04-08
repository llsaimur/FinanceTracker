namespace FinanceTrackerUI.Models.Responses
{
    public class BudgetSummaryResponse
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSpent { get; set; }
        public double PercentageUsed { get; set; }
        public string Status { get; set; }
    }

}
