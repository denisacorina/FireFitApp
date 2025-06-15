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
        [Range(30, 300, ErrorMessage = "Starting weight must be between 30 and 300 kg")]
        public decimal StartingWeight { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public ActivityLevel ActivityLevel { get; set; }

        [Required]
        public WeightChangeType WeightGoal { get; set; }

        [Required]
        public ExperienceLevel FitnessExperience { get; set; }

        [Required]
        public List<DietaryPreference> DietaryPreferences { get; set; } = new();
        [Required]
        public List<WorkoutType> WorkoutTypes { get; set; } = new();    
        
        [Required]
        [Range(1000, 6000, ErrorMessage = "Daily Calorie Goal must be between 1000 and 6000 kcal")]
        public int DailyCalorieGoal { get; set; } = new();

        public float ProteinGoal { get; set; }
        public float CarbsGoal { get; set; }
        public float FatsGoal { get; set; }
    }
} 