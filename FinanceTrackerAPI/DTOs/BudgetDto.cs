using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerAPI.DTOs
{
    public class BudgetDto
    {
        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100, ErrorMessage = "Category cannot be longer than 100 characters.")]
        public string Category { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be greater than zero.")]
        public decimal Limit { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DateGreaterThan("StartDate", ErrorMessage = "End date must be later than start date.")]
        public DateTime EndDate { get; set; }
    }

    // Custom Validation Attribute to check that the EndDate is later than the StartDate
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDate = validationContext.ObjectType.GetProperty(_comparisonProperty).GetValue(validationContext.ObjectInstance, null);

            if (startDate != null && value != null && (DateTime)value <= (DateTime)startDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
