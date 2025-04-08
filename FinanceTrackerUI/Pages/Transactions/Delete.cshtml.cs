using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Transactions
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DeleteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public TransactionResponse? Transaction { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null) return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var response = await _httpClient.GetAsync($"{apiBase}/api/Transactions/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Transaction = JsonConvert.DeserializeObject<TransactionResponse>(content);
                }
                else
                {
                    ErrorMessage = "Transaction not found or access denied.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error fetching transaction: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null) return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var response = await _httpClient.DeleteAsync($"{apiBase}/api/Transactions/{id}");

                if (response.IsSuccessStatusCode)
                    return RedirectToPage("/Transactions/Index");

                ErrorMessage = "Failed to delete transaction.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting transaction: {ex.Message}";
            }

            return await OnGetAsync(id);
        }
    }
}
