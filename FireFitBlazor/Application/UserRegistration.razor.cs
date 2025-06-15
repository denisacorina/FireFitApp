using System.ComponentModel.DataAnnotations;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using Microsoft.AspNetCore.Components;
using Radzen;
using FireFitBlazor.Domain.ContextInterfaces;
using NotificationService = Radzen.NotificationService;
using Microsoft.AspNetCore.Authentication;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using FireFitBlazor.Domain.ValueObjects;
using FireFitBlazor.Application.Controllers;
using FireFitBlazor.Domain.Models;
using System.Net.Http;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Enums;

namespace FireFitBlazor.Application
{
    public partial class UserRegistration : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; } = default!;

        [Inject]
        private IAuthenticationService AuthenticationService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private DialogService DialogService { get; set; } = default!;

        [Inject]
        private NotificationService NotificationService { get; set; } = default!;
        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; } = default!;

        [Inject]
        private IGoalService GoalService { get; set; } = default!;

        public UserRegistrationModel FormModel { get; set; } = new();

        public string Message = "";
        public bool IsError = false;
        public bool IsLoading = false;
        public int CurrentStep = 1;
        public int ProteinGrams => CalculateProteinGoal();
        public int CarbsGrams => (int)((DailyCalories - (ProteinGrams * 4) - (FatsGrams * 9)) / 4); 
        public int FatsGrams => (int)((DailyCalories * 0.3) / 9); 
        public IEnumerable<object> GoalIntensityOptions =>
    Enum.GetValues<GoalIntensity>()
        .Cast<GoalIntensity>()
        .Select(i => new { Key = i.ToString(), Value = i });
        public IEnumerable<KeyValuePair<string, Gender>> Genders =>
         Enum.GetValues<Gender>()
         .Cast<Gender>()
         .Select(g => new KeyValuePair<string, Gender>(g.ToString(), g));


        public IEnumerable<KeyValuePair<string, ActivityLevel>> ActivityLevels =>
      Enum.GetValues<ActivityLevel>()
      .Cast<ActivityLevel>()
      .Select(g => new KeyValuePair<string, ActivityLevel>(g.ToString(), g));

        public IEnumerable<KeyValuePair<string, WeightChangeType>> WeightGoals =>
    Enum.GetValues<WeightChangeType>()
    .Cast<WeightChangeType>()
    .Select(g => new KeyValuePair<string, WeightChangeType>(g.ToString(), g));

        public List<DietaryPreference> SelectedDietary { get; set; } = new List<DietaryPreference>();
        public List<WorkoutType> SelectedWorkoutTypes { get; set; } = new List<WorkoutType>();

        public IEnumerable<DietaryPreference>? DietaryPreferences { get; set; }
        public IEnumerable<WorkoutType>? WorkoutTypes { get; set; }
        public GoalIntensity GoalIntensity { get; set; } = GoalIntensity.Moderate;

        public void UpdateSelectedDietary()
        {
            if (FormModel.SelectedDietary != null)
            {
                SelectedDietary = FormModel.SelectedDietary.ToList();
            }
        }

        public void UpdateSelectedWorkoutTypes()
        {
            if (FormModel.SelectedWorkoutTypes != null)
            {
                SelectedWorkoutTypes = FormModel.SelectedWorkoutTypes.ToList();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DietaryPreferences = Enum.GetValues(typeof(DietaryPreference)).Cast<DietaryPreference>();
            WorkoutTypes = Enum.GetValues(typeof(WorkoutType)).Cast<WorkoutType>();
            FormModel.SelectedDietary = new List<DietaryPreference>();
            FormModel.SelectedWorkoutTypes = new List<WorkoutType>();
            UpdateSelectedDietary();
            UpdateSelectedWorkoutTypes();
            UseCalorieOverride = false;
        }

        public void OnGenderChange(object value)
        {
            FormModel.Gender = (Gender)value;
            Console.WriteLine($"Gender selected: {FormModel.Gender}");
        }

        public void OnActivityLevelChange(object value)
        {
            FormModel.ActivityLevel = (ActivityLevel)value;
            Console.WriteLine($"Activity Level selected: {FormModel.ActivityLevel}");
        }
        public void OnWeightChangeTypeChange(object value)
        {
            FormModel.WeightGoal = (WeightChangeType)value;
            Console.WriteLine($"Weight Goal selected: {FormModel.WeightGoal}");
        }

        public double GetWeeklyRate() => FormModel.GoalIntensity switch
        {
            GoalIntensity.Extreme => 1.0,
            GoalIntensity.Moderate => 0.75,
            GoalIntensity.Slow => 0.5,
            _ => 0.75
        };

        public string GetWeeklyChangeText()
        {
            var rate = GetWeeklyRate();
            return FormModel.WeightGoal switch
            {
                WeightChangeType.Lose => $"Lose ~{rate} kg/week",
                WeightChangeType.Gain => $"Gain ~{rate} kg/week",
                _ => "Maintain current weight"
            };
        }

        public double CalculateBMR()
        {
            return FormModel.Gender == Gender.Male
                ? 10 * (double)FormModel.CurrentWeight + 6.25 * FormModel.Height - 5 * FormModel.Age + 5
                : 10 * (double)FormModel.CurrentWeight + 6.25 * FormModel.Height - 5 * FormModel.Age - 161;
        }

        public int EstimatedWeeks
        {
            get
            {
                if (FormModel.CurrentWeight <= 0 || FormModel.TargetWeight <= 0 || FormModel.Age <= 0 || FormModel.Height <= 0)
                    return 0;

                if (FormModel.WeightGoal == WeightChangeType.Maintain)
                    return 0;

                var current = (double)FormModel.CurrentWeight;
                var target = (double)FormModel.TargetWeight;
                var diff = Math.Abs(current - target);

                double bmr = CalculateBMR();
                double multiplier = FormModel.ActivityLevel switch
                {
                    ActivityLevel.Sedentary => 1.2,
                    ActivityLevel.Light => 1.375,
                    ActivityLevel.Moderate => 1.55,
                    ActivityLevel.Active => 1.725,
                    ActivityLevel.VeryActive => 1.9,
                    _ => 1.2
                };
                double maintenance = bmr * multiplier;

                double adjustedCalories = UseCalorieOverride && ManualCalorieOverride >= 1000
                    ? ManualCalorieOverride
                    : CalculateDailyCalories(bmr, multiplier); // reuse values

                double dailyChangeKcal = FormModel.WeightGoal switch
                {
                    WeightChangeType.Lose => maintenance - adjustedCalories,
                    WeightChangeType.Gain => adjustedCalories - maintenance,
                    _ => 0
                };

                double kgPerDay = Math.Round(dailyChangeKcal / 7700.0, 3);
                if (kgPerDay <= 0.005)
                    return 0;

                return (int)Math.Ceiling(diff / (kgPerDay * 7));
            }
        }


        public double CalculateDailyCalories(double? bmrOverride = null, double? multiplierOverride = null)
        {
            double bmr = bmrOverride ?? CalculateBMR();
            double multiplier = multiplierOverride ?? FormModel.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2,
                ActivityLevel.Light => 1.375,
                ActivityLevel.Moderate => 1.55,
                ActivityLevel.Active => 1.725,
                ActivityLevel.VeryActive => 1.9,
                _ => 1.2
            };

            double maintenance = bmr * multiplier;
            double adjustment = GetWeeklyRate() * 7700 / 7.0;

            return FormModel.WeightGoal switch
            {
                WeightChangeType.Lose => Math.Max(maintenance - adjustment, 1000),
                WeightChangeType.Gain => maintenance + adjustment,
                _ => maintenance
            };
        }

        public int ManualCalorieOverride { get; set; } = 0; // value when override is active
        public bool UseCalorieOverride { get; set; } = false; // default: use calculated
                                                              //public int DailyCalories => (int)Math.Max(CalculateDailyCalories(), 0); 
        public int DailyCalories =>
      UseCalorieOverride && ManualCalorieOverride >= 1000
          ? ManualCalorieOverride
          : (int)Math.Round(CalculateDailyCalories());

        public void OnDietaryPreferenceChanged(DietaryPreference pref, object checkedValue)
        {
            bool isChecked = checkedValue is bool b && b;
            if (isChecked)
            {
                if (!FormModel.SelectedDietary.Contains(pref))
                    FormModel.SelectedDietary.Add(pref);
            }
            else
                FormModel.SelectedDietary.Remove(pref);

            UpdateSelectedDietary();
        }

        public void OnWorkoutTypeChanged(WorkoutType pref, object checkedValue)
        {
            bool isChecked = checkedValue is bool b && b;
            if (isChecked)
            {
                if (!FormModel.SelectedWorkoutTypes.Contains(pref))
                    FormModel.SelectedWorkoutTypes.Add(pref);
            }
            else
                FormModel.SelectedWorkoutTypes.Remove(pref);

            UpdateSelectedWorkoutTypes();
        }

        protected void OnWorkoutPreferenceChanged(WorkoutType type, bool selected)
        {
            if (selected && !FormModel.SelectedWorkoutTypes.Contains(type))
                FormModel.SelectedWorkoutTypes.Add(type);
            else if (!selected && FormModel.SelectedWorkoutTypes.Contains(type))
                FormModel.SelectedWorkoutTypes.Remove(type);
        }

        public void OnManualCalorieChange(int value)
        {
            if (value != -1 && value < 1000)
            {
                ManualCalorieOverride = 1000;
            }
            else
            {
                ManualCalorieOverride = value;
                UseCalorieOverride = true;
            }

            StateHasChanged();
        }

        protected void OnDietaryPreferenceChanged(DietaryPreference pref, bool selected)
        {
            if (selected && !FormModel.SelectedDietary.Contains(pref))
                FormModel.SelectedDietary.Add(pref);
            else if (!selected && FormModel.SelectedDietary.Contains(pref))
                FormModel.SelectedDietary.Remove(pref);
        }

        public void NextStep()
        {
            if (CurrentStep < 6)
            {
                CurrentStep++;
            }
        }

        public void PreviousStep()
        {
            if (CurrentStep > 1)
            {
                CurrentStep--;
            }
        }

        public async void CreateUser()
        {
            IsLoading = true;
            Message = "";
            IsError = false;

            try
            {
                var dto = new RegisterDto
                {
                    Email = FormModel.Email,
                    Name = FormModel.Name,
                    Password = FormModel.Password,
                    Age = FormModel.Age,
                    Height = FormModel.Height,
                    StartingWeight = FormModel.CurrentWeight,
                    CurrentWeight = FormModel.CurrentWeight,
                    TargetWeight = FormModel.TargetWeight,
                    Gender = FormModel.Gender,
                    ActivityLevel = FormModel.ActivityLevel,
                    WeightGoal = FormModel.WeightGoal,
                    DietaryPreferences = FormModel.SelectedDietary,
                    WorkoutTypes = FormModel.SelectedWorkoutTypes,
                    DailyCalorieGoal = DailyCalories,
                    ProteinGoal = ProteinGrams,
                    CarbsGoal = CarbsGrams,
                    FatsGoal = FatsGrams
                };
                var http = HttpClientFactory.CreateClient("ServerAPI");

                var response = await http.PostAsJsonAsync("/api/customauth/register", dto);

                if (response.IsSuccessStatusCode)
                {
                    // Get the created user's ID from the response
                    var user = await response.Content.ReadFromJsonAsync<User>();
                    if (user != null)
                    {
                        // Create initial goal
                        var goalType = FormModel.WeightGoal switch
                        {
                            WeightChangeType.Lose => GoalType.WeightLoss,
                            WeightChangeType.Gain => GoalType.WeightGain,
                            _ => GoalType.Maintenance
                        };

                        // Calculate target date (e.g., 12 weeks from now)
                        var targetDate = DateTime.UtcNow.AddDays(EstimatedWeeks * 7);

                        await GoalService.CreateGoalAsync(
                            userId: user.UserId,
                            type: goalType,
                            calorieGoal: DailyCalories,
                            proteinGoal: ProteinGrams,
                            carbGoal: CarbsGrams,
                            fatGoal: FatsGrams,
                            intermittentFasting: false, 
                            fastingWindow: 0,
                            targetWeight: FormModel.TargetWeight,
                            targetDate: targetDate
                        );

                        Message = "Registration successful. Redirecting to login...";
                        NavigationManager.NavigateTo("/login", forceLoad: true);
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    IsError = true;
                    Message = $"Registration failed: {error}";
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                Message = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }



        public void OnGoogleRegister()
        {
            NavigationManager.NavigateTo("/api/account/external-login?provider=Google");
        }
        public void ResetToRecommended()
        {
            ManualCalorieOverride = 0;   
            UseCalorieOverride = false;  
            StateHasChanged();           
        }


        public class UserRegistrationModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email
            {
                get; set;
            }

            [Required(ErrorMessage = "Password is required")]
            [Range(6, 100, ErrorMessage = "Password must be at least 6 characters long")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Age is required")]
            [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Height is required")]
            [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm")]
            public int Height { get; set; }

            [Required(ErrorMessage = "Current weight is required")]
            [Range(30, 300, ErrorMessage = "Weight must be between 30 and 300 kg")]
            public decimal CurrentWeight { get; set; }

            [Required(ErrorMessage = "Target weight is required")]
            [Range(30, 300, ErrorMessage = "Weight must be between 30 and 300 kg")]
            public decimal TargetWeight { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            public Gender Gender { get; set; }

            [Required(ErrorMessage = "Activity level is required")]
            public ActivityLevel ActivityLevel { get; set; }

            [Required(ErrorMessage = "Weight goal is required")]
            public WeightChangeType WeightGoal { get; set; }

            [Required(ErrorMessage = "Goal intensity is required")]
            public GoalIntensity GoalIntensity { get; set; }

            public Dictionary<DietaryPreference, bool>? DietaryPreferences { get; set; } = new();
            public List<DietaryPreference> SelectedDietary { get; set; } = new();
            public Dictionary<WorkoutType, bool>? WorkoutTypes { get; set; } = new();
            public List<WorkoutType>? SelectedWorkoutTypes { get; set; } = new();
            public UserRegistrationModel()
            {
                foreach (var preference in Enum.GetValues<DietaryPreference>())
                {
                    DietaryPreferences[preference] = false;
                }

                foreach (var workoutType in Enum.GetValues<WorkoutType>())
                {
                    WorkoutTypes[workoutType] = false;
                }
            }
        }

        private int CalculateProteinGoal()
        {
            // Base protein calculation: 1.6g per kg of bodyweight for general fitness
            double baseProtein = (double)FormModel.CurrentWeight * 1.6;

            double activityMultiplier = FormModel.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.0,
                ActivityLevel.Light => 1.1,
                ActivityLevel.Moderate => 1.2,
                ActivityLevel.Active => 1.3,
                ActivityLevel.VeryActive => 1.4,
                _ => 1.0
            };

            double goalMultiplier = FormModel.WeightGoal switch
            {
                WeightChangeType.Lose => 1.2, // Higher protein for weight loss to preserve muscle
                WeightChangeType.Gain => 1.3, // Higher protein for muscle gain
                _ => 1.0 // Maintenance
            };

            double finalProtein = baseProtein * activityMultiplier * goalMultiplier;

            return Math.Max((int)Math.Round(finalProtein), 50);
        }
    }
}