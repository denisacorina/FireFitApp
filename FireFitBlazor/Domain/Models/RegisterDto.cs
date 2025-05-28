using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

namespace FireFitBlazor.Domain.Models
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        [Required]
        [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm")]
        public int Height { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Current weight must be between 30 and 300 kg")]
        public decimal CurrentWeight { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Target weight must be between 30 and 300 kg")]
        public decimal TargetWeight { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public ActivityLevel ActivityLevel { get; set; }

        [Required]
        public WeightChangeType WeightGoal { get; set; }

        public List<DietaryPreference> DietaryPreferences { get; set; } = new();
    }
} 