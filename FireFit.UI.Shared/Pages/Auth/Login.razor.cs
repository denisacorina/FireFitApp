using System.ComponentModel.DataAnnotations;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFit.UI.Shared.Authentication;
using Microsoft.AspNetCore.Components;

namespace FireFit.UI.Shared.Pages.Auth;

public partial class Login : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private SharedAuthStateProvider AuthStateProvider { get; set; } = default!;

    private readonly LoginModel loginModel = new();
    private string message = string.Empty;
    private bool isError;
    private bool isLoading;

    private async Task HandleValidSubmit()
    {
        if (isLoading)
        {
            return;
        }

        message = string.Empty;
        isError = false;
        isLoading = true;

        try
        {
            var loginDto = new LoginDto
            {
                Email = loginModel.Email,
                Password = loginModel.Password,
                RememberMe = loginModel.RememberMe
            };

            var user = await AuthService.LoginAsync(loginDto);
            if (user is not null)
            {
                AuthStateProvider.NotifyUserChanged(user);
                message = "Login successful. Redirecting...";
                Navigation.NavigateTo("/dashboard", forceLoad: false);
            }
            else
            {
                isError = true;
                message = "Invalid login. Please check your credentials.";
            }
        }
        catch
        {
            isError = true;
            message = "Unable to sign in right now. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnGoogleLogin()
    {
        Navigation.NavigateTo("/api/account/external-login?provider=Google", forceLoad: true);
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
