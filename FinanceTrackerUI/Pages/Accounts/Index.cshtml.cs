using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public List<AccountResponse> Accounts { get; set; } = new();
        public decimal TotalBalance => Accounts.Sum(a => a.Balance);
        public decimal TotalSpent { get; set; }
        public decimal TotalBudget { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var token = GetToken();
            if (token == null)
                return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var apiBase = _configuration["ApiBaseUrl"];

                // Fetch accounts
                var accRes = await _httpClient.GetAsync($"{apiBase}/api/Accounts");
                if (accRes.IsSuccessStatusCode)
                {
                    var content = await accRes.Content.ReadAsStringAsync();
                    Accounts = JsonConvert.DeserializeObject<List<AccountResponse>>(content) ?? new();
                }

                // Fetch transactions (for total spent)
                var txRes = await _httpClient.GetAsync($"{apiBase}/api/Transactions");
                if (txRes.IsSuccessStatusCode)
                {
                    var content = await txRes.Content.ReadAsStringAsync();
                    var txs = JsonConvert.DeserializeObject<List<TransactionResponse>>(content);
                    TotalSpent = txs?.Sum(t => t.Amount) ?? 0;
                }

                // Fetch budgets (for budget goal)
                var budgetRes = await _httpClient.GetAsync($"{apiBase}/api/Budgets");
                if (budgetRes.IsSuccessStatusCode)
                {
                    var content = await budgetRes.Content.ReadAsStringAsync();
                    var budgets = JsonConvert.DeserializeObject<List<BudgetResponse>>(content);
                    TotalBudget = budgets?.Sum(b => b.Limit) ?? 0;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private string? GetToken()
        {
            return HttpContext.Session.GetString("auth_token");
        }
    }
}
