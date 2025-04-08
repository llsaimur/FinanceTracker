using Newtonsoft.Json;

namespace FinanceTrackerUI.Models.Requests
{
    public class UpdateAccountRequest
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public string AccountId { get; set; }
    }
}
