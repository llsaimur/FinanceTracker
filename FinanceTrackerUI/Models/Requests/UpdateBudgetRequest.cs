using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerUI.Models.Requests
{
    public class UpdateBudgetRequest
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
