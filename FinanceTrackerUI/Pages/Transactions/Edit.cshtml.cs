using FinanceTrackerUI.Models.Requests;
using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTrackerUI.Pages.Transactions
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [BindProperty]
        public UpdateTransactionRequest Transaction { get; set; } = new();

        public List<AccountResponse> UserAccounts { get; set; } = new();
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

                var transactionResult = await _httpClient.GetAsync($"{apiBase}/api/Transactions/{id}");
                if (transactionResult.IsSuccessStatusCode)
                {
                    var transactionJson = await transactionResult.Content.ReadAsStringAsync();
                    var transaction = JsonConvert.DeserializeObject<TransactionResponse>(transactionJson);
                    if (transaction != null)
                    {
                        Transaction.Id = transaction.Id;
                        Transaction.AccountId = transaction.AccountId;
                        Transaction.Amount = transaction.Amount;
                        Transaction.Category = transaction.Category;
                        Transaction.Date = transaction.Date;
                        Transaction.Description = transaction.Description;
                    }
                }

                var accountResult = await _httpClient.GetAsync($"{apiBase}/api/Accounts");
                if (accountResult.IsSuccessStatusCode)
                {
                    var accountJson = await accountResult.Content.ReadAsStringAsync();
                    UserAccounts = JsonConvert.DeserializeObject<List<AccountResponse>>(accountJson) ?? new();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("auth_token");
            if (token == null) return RedirectToPage("/Accounts/Login");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var apiBase = _configuration["ApiBaseUrl"];

                var json = JsonConvert.SerializeObject(Transaction);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{apiBase}/api/Transactions/{Transaction.Id}", content);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("/Accounts/Index");

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

                    return await OnGetAsync(Transaction.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return await OnGetAsync(Transaction.Id);
        }
    }
}
