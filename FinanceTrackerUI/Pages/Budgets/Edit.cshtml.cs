using FinanceTrackerUI.Models.Requests;
using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTrackerUI.Pages.Budgets
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
        public UpdateBudgetRequest Budget { get; set; } = new();

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
                var response = await _httpClient.GetAsync($"{apiBase}/api/Budgets/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var existing = JsonConvert.DeserializeObject<BudgetResponse>(json);

                    if (existing != null)
                    {
                        Budget.Id = existing.Id;
                        Budget.Category = existing.Category;
                        Budget.Limit = existing.Limit;
                        Budget.StartDate = existing.StartDate;
                        Budget.EndDate = existing.EndDate;
                    }
                }
                else
                {
                    ErrorMessage = "Unable to fetch budget.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
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
                var json = JsonConvert.SerializeObject(Budget);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{apiBase}/api/Budgets/{Budget.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToPage("/Budgets/Index");
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

                    return await OnGetAsync(Budget.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return await OnGetAsync(Budget.Id);
        }

    }
}
