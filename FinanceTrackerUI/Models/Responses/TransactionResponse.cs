namespace FinanceTrackerUI.Models.Responses
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
