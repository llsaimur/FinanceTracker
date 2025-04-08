using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerAPI.DTOs
{
    public class AccountDto
    {
        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, ErrorMessage = "Account name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Account type is required.")]
        [StringLength(50, ErrorMessage = "Account type cannot be longer than 50 characters.")]
        public string Type { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Balance must be greater than 0.")]
        public decimal Balance { get; set; }
    }
}
