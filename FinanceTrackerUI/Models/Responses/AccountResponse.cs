using Newtonsoft.Json;

namespace FinanceTrackerUI.Models.Responses
{
    public class AccountResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
    }
}
