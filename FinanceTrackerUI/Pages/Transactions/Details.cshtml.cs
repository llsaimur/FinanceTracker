using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Transactions
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public TransactionResponse? Transaction { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null)
                return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];

                // 1. Fetch the transaction by ID
                var response = await _httpClient.GetAsync($"{apiBase}/api/Transactions/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Transaction not found or access denied.";
                    return Page();
                }

                var content = await response.Content.ReadAsStringAsync();
                Transaction = JsonConvert.DeserializeObject<TransactionResponse>(content);

                // 2. Fetch the related account to get the name
                if (Transaction != null && !string.IsNullOrWhiteSpace(Transaction.AccountId))
                {
                    var accountResponse = await _httpClient.GetAsync($"{apiBase}/api/Accounts/{Transaction.AccountId}");

                    if (accountResponse.IsSuccessStatusCode)
                    {
                        var accountJson = await accountResponse.Content.ReadAsStringAsync();
                        var account = JsonConvert.DeserializeObject<AccountResponse>(accountJson);
                        Transaction.AccountName = account?.Name ?? "(Unknown)";
                    }
                    else
                    {
                        Transaction.AccountName = "(Account not found)";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error fetching transaction: {ex.Message}";
            }

            return Page();
        }

    }
}
