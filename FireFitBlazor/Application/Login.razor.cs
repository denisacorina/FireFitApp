using System.ComponentModel.DataAnnotations;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace FireFitBlazor.Application
{
    public partial class Login
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        private DialogService DialogService { get; set; } = default!;

        [Inject]
        public HttpClient Http { get; set; } = default!;

        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; } = default!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = default!;

        private LoginModel UserLogin = new();
        private string Message = "";
        private bool IsError = false;
        private bool IsLoading = false; 
        [Inject] private IJSRuntime JS { get; set; } = default!;

      
        
        private async Task HandleValidSubmit()
        {
            IsLoading = true;
            IsError = false;

            LoginModel loginDto = new()
            {
                Email = UserLogin.Email,
                Password = UserLogin.Password,
                RememberMe = UserLogin.RememberMe
            };

            //var response = await Http.PostAsJsonAsync("/api/customauth/login", loginDto);
            var http = HttpClientFactory.CreateClient("ServerAPI");

            var response = await http.PostAsJsonAsync("/api/customauth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                Message = "Login successful. Redirecting...";
                NavigationManager.NavigateTo("/my-profile", forceLoad: true);
            }
            else
            {
                IsError = true;
                Message = "Invalid login. Please check your credentials.";
            }

            IsLoading = false;
        }


        private void OnGoogleLogin()
        {
            NavigationManager.NavigateTo("/api/account/external-login?provider=Google");
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = "";

            public bool RememberMe { get; set; }
        }
    }
} 