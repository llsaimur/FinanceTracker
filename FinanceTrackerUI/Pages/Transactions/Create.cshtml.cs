using FinanceTrackerUI.Models.Requests;
using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTrackerUI.Pages.Transactions
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [BindProperty]
        public CreateTransactionRequest Transaction { get; set; } = new();

        public List<AccountResponse> UserAccounts { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null)
                return RedirectToPage("/Accounts/Login");

            Transaction.Date = DateTime.Today;

            await LoadUserAccountsAsync();

            return Page();
        }

        private async Task LoadUserAccountsAsync()
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null) return;

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var response = await _httpClient.GetAsync($"{apiBase}/api/Accounts");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var accounts = JsonConvert.DeserializeObject<List<AccountResponse>>(content);
                    if (accounts != null) UserAccounts = accounts;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading accounts: {ex.Message}";
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null)
                return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiBase = _configuration["ApiBaseUrl"];
                var json = JsonConvert.SerializeObject(Transaction);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{apiBase}/api/Transactions", content);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Transaction added successfully.";
                    ModelState.Clear();
                    return RedirectToPage("/Transactions/Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    var matches = Regex.Matches(errorContent, "\"([^\"]+)\":\\[\"([^\"]+)\"\\]");
                    if (matches.Count > 0)
                    {
                        var errors = matches.Select(m => $"{m.Groups[1].Value}: {m.Groups[2].Value}");
                        ErrorMessage = string.Join("<br>", errors);
                    }
                    else
                    {
                        try
                        {
                            ErrorMessage = errorContent.ToString();
                        }
                        catch
                        {
                            ErrorMessage = "Something went wrong. Please try again.";
                        }
                    }

                    await LoadUserAccountsAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                await LoadUserAccountsAsync();
                ErrorMessage = $"Error: {ex.Message}";
            }

            return await OnGetAsync();
        }
    }
}
