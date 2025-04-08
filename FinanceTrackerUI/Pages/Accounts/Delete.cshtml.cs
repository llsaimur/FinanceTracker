using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages.Account
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

        [BindProperty]
        public AccountResponse Account { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            var token = GetToken();
            if (token == null)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var response = await _httpClient.GetAsync($"{apiBase}/api/Accounts/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<AccountResponse>(content);
                    if (account != null)
                    {
                        Account = account;
                    }
                }
                else
                {
                    ErrorMessage = "Unable to load account details.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = GetToken();
            if (token == null)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var response = await _httpClient.DeleteAsync($"{apiBase}/api/Accounts/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Accounts/Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Error deleting account: {errorContent}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        private string? GetToken()
        {
            return HttpContext.Session.GetString("auth_token");
        }
    }
}
