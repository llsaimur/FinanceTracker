using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerAPI.DTOs
{
    public class TransactionDto
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public string AccountId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100, ErrorMessage = "Category cannot be longer than 100 characters.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }
    }
}
