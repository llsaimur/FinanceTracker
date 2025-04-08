using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Budgets
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

        public BudgetResponse? Budget { get; set; }
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
                var response = await _httpClient.GetAsync($"{apiBase}/api/Budgets/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Budget = JsonConvert.DeserializeObject<BudgetResponse>(json);
                }
                else
                {
                    ErrorMessage = "Budget not found or access denied.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error fetching budget: {ex.Message}";
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
                var response = await _httpClient.DeleteAsync($"{apiBase}/api/Budgets/{id}");

                if (response.IsSuccessStatusCode)
                    return RedirectToPage("/Budgets/Index");

                ErrorMessage = "Failed to delete budget.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting budget: {ex.Message}";
            }

            return await OnGetAsync(id); // Refresh budget details if deletion fails
        }
    }
}
