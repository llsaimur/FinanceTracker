using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerUI.Models.Requests
{
    public class CreateBudgetRequest
    {
        public string Category { get; set; } = string.Empty;
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
