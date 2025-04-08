using FinanceTrackerUI.Models;
using FinanceTrackerUI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTrackerUI.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RegisterModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [BindProperty]
        public RegisterRequest RegisterRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var apiBase = _configuration["ApiBaseUrl"];
                var json = JsonConvert.SerializeObject(RegisterRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{apiBase}/api/Users/register", content);
                //var response = await _httpClient.PostAsJsonAsync($"{apiBase}/api/Users/register", RegisterRequest);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Registration successful!";
                    return RedirectToPage("/Accounts/Login");
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
                ErrorMessage = "An error occurred: " + ex.Message;
                return Page();
            }
        }

    }
}
