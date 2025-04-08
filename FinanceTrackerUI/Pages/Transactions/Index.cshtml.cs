using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Transactions
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

        public List<TransactionResponse> Transactions { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null) return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];

                // Step 1: Fetch Transactions
                var transactionResponse = await _httpClient.GetAsync($"{apiBase}/api/Transactions");

                if (!transactionResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Error loading transactions: {await transactionResponse.Content.ReadAsStringAsync()}";
                    return Page();
                }

                var transactionJson = await transactionResponse.Content.ReadAsStringAsync();
                var transactionList = JsonConvert.DeserializeObject<List<TransactionResponse>>(transactionJson) ?? new();

                // Step 2: For each transaction, get its account details
                foreach (var transaction in transactionList)
                {
                    var accountResponse = await _httpClient.GetAsync($"{apiBase}/api/Accounts/{transaction.AccountId}");
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        var accountJson = await accountResponse.Content.ReadAsStringAsync();
                        var account = JsonConvert.DeserializeObject<AccountResponse>(accountJson);
                        transaction.AccountName = account?.Name ?? "(Unknown)";
                    }
                    else
                    {
                        transaction.AccountName = "(Account not found)";
                    }
                }

                Transactions = transactionList;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }

            return Page();
        }
    }
}
