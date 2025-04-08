using FinanceTrackerUI.Models;
using FinanceTrackerUI.Models.Requests;
using FinanceTrackerUI.Models.Responses;  // Include the LoginResponse model
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace FinanceTrackerUI.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [BindProperty]
        public LoginRequest LoginRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var apiBase = _configuration["ApiBaseUrl"];

                var json = JsonConvert.SerializeObject(LoginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{apiBase}/api/Users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);

                    HttpContext.Session.SetString("auth_token", loginResponse.Token);

                    SuccessMessage = loginResponse.Message;
                    return RedirectToPage("/Accounts/Index"); 
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);

                        ErrorMessage = !string.IsNullOrWhiteSpace(loginResponse?.Message)
                            ? loginResponse.Message
                            : "Login failed. Please check your credentials.";
                    }
                    catch
                    {
                        ErrorMessage = responseString;
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
