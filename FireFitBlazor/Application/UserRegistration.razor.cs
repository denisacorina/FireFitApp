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

        public UserRegistrationModel FormModel { get; set; } = new();

        public string Message = "";
        public bool IsError = false;
        public bool IsLoading = false;
        public int CurrentStep = 1;
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

        public IEnumerable<DietaryPreference>? DietaryPreferences { get; set; }

        public void UpdateSelectedDietary()
        {
            if (FormModel.SelectedDietary != null)
            {
                SelectedDietary = FormModel.SelectedDietary.ToList();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DietaryPreferences = Enum.GetValues(typeof(DietaryPreference)).Cast<DietaryPreference>();
            FormModel.SelectedDietary = new List<DietaryPreference>();
            UpdateSelectedDietary();
        }

        public void OnGenderChange(object value)
        {
            // Handle the change when gender selection changes
            FormModel.Gender = (Gender)value;
            Console.WriteLine($"Gender selected: {FormModel.Gender}");
        }

        public void OnActivityLevelChange(object value)
        {
            // Handle the change when activity level selection changes
            FormModel.ActivityLevel = (ActivityLevel)value;
            Console.WriteLine($"Activity Level selected: {FormModel.ActivityLevel}");
        }

        public void OnDietaryPreferenceChanged(DietaryPreference pref, object checkedValue)
        {
            bool isChecked = checkedValue is bool b && b;
            if (isChecked)
            {
                if (!FormModel.SelectedDietary.Contains(pref))
                    FormModel.SelectedDietary.Add(pref);
            }
            else
            {
                if (FormModel.SelectedDietary.Contains(pref))
                    FormModel.SelectedDietary.Remove(pref);
            }
            UpdateSelectedDietary();
        }


        public void NextStep()
        {
            if (CurrentStep < 3)
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
                    CurrentWeight = FormModel.CurrentWeight,
                    TargetWeight = FormModel.TargetWeight,
                    Gender = FormModel.Gender,
                    ActivityLevel = FormModel.ActivityLevel,
                    WeightGoal = FormModel.WeightGoal,
                    DietaryPreferences = SelectedDietary
                };
                var http = HttpClientFactory.CreateClient("ServerAPI");

                var response = await http.PostAsJsonAsync("/api/customauth/register", dto);
                //var response = await Http.PostAsJsonAsync("/api/customauth/register", dto);

                if (response.IsSuccessStatusCode)
                {
                    Message = "Registration successful. Redirecting to login...";
                    NavigationManager.NavigateTo("/login", forceLoad: true);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    IsError = true;
                    Message = $"Registration failed: {error}";
                }

                //var result = CreateUserContext.Execute(
                //    FormModel.Email,
                //    FormModel.Password,
                //    FormModel.Name,
                //    FormModel.Age,
                //    FormModel.Height,
                //    FormModel.CurrentWeight,
                //    FormModel.TargetWeight,
                //    FormModel.Gender,
                //    FormModel.ActivityLevel,
                //    FormModel.WeightGoal,
                //    SelectedDietary
                //);
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

            public Dictionary<DietaryPreference, bool>? DietaryPreferences { get; set; } = new();
            public List<DietaryPreference> SelectedDietary { get; set; } = new List<DietaryPreference>();

            public UserRegistrationModel()
            {
                foreach (var preference in Enum.GetValues<DietaryPreference>())
                {
                    DietaryPreferences[preference] = false;
                }
            }
        }
    }
}