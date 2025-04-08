using FinanceTrackerUI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FinanceTrackerUI.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    public List<BudgetSummary> BudgetSummaries { get; set; } = new();
    public decimal TotalSpent { get; set; }
    public decimal TotalBudget { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("auth_token");
        if (token == null)
        {
            return Page();
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var apiBase = _configuration["ApiBaseUrl"];

            List<TransactionResponse> transactions = new();
            List<BudgetResponse> budgets = new();

            // Fetch transactions
            var txRes = await _httpClient.GetAsync($"{apiBase}/api/Transactions");
            if (txRes.IsSuccessStatusCode)
            {
                var txJson = await txRes.Content.ReadAsStringAsync();
                transactions = JsonConvert.DeserializeObject<List<TransactionResponse>>(txJson) ?? new();
                TotalSpent = transactions.Sum(t => t.Amount);
            }

            // Fetch budgets
            var budgetRes = await _httpClient.GetAsync($"{apiBase}/api/Budgets");
            if (budgetRes.IsSuccessStatusCode)
            {
                var budgetJson = await budgetRes.Content.ReadAsStringAsync();
                budgets = JsonConvert.DeserializeObject<List<BudgetResponse>>(budgetJson) ?? new();
                TotalBudget = budgets.Sum(b => b.Limit);
            }

            // Build summaries per budget
            BudgetSummaries = budgets.Select(b =>
            {
                var spent = transactions
                    .Where(t => t.Category.Equals(b.Category, StringComparison.OrdinalIgnoreCase)
                                && t.Date >= b.StartDate && t.Date <= b.EndDate)
                    .Sum(t => t.Amount);

                return new BudgetSummary
                {
                    Category = b.Category,
                    Limit = b.Limit,
                    Spent = spent
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching home summary: " + ex.Message);
        }

        return Page();
    }

    public class BudgetSummary
    {
        public string Category { get; set; } = string.Empty;
        public decimal Limit { get; set; }
        public decimal Spent { get; set; }
        public double PercentUsed => Limit > 0 ? (double)(Spent / Limit) * 100 : 0;
        public string Status =>
            PercentUsed >= 100 ? "Exceeded" :
            PercentUsed >= 80 ? "Near Limit" :
            "On Track";
    }
}
