using FinanceTrackerUI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTrackerUI.Pages.Budgets
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
        public CreateBudgetRequest Budget { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            Budget.StartDate = DateTime.Today;
            Budget.EndDate = DateTime.Today.AddMonths(1);
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

                var response = await _httpClient.PostAsync($"{apiBase}/api/Budgets", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Budgets/Index");
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

                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

    }
}
