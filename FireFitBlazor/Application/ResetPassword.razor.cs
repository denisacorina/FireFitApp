//using System.ComponentModel.DataAnnotations;
//using FireFitBlazor.Domain.Services;
//using Microsoft.AspNetCore.Components;


//namespace FireFitBlazor.Application
//{
//    public partial class ResetPassword
//    {
//        [Parameter]
//        [SupplyParameterFromQuery]
//        public string? Email { get; set; }

//        [Parameter]
//        [SupplyParameterFromQuery]
//        public string? Token { get; set; }

//        private ResetPasswordModel UserResetPassword = new();
//        public string Message = "";
//        public bool IsError = false;
//        public bool IsLoading = false;

//        protected override void OnInitialized()
//        {
//            if (!string.IsNullOrEmpty(Email))
//            {
//                UserResetPassword.Email = Email;
//            }
//        }

//        public async Task HandleValidSubmit()
//        {
//            IsLoading = true;
//            Message = "";
//            IsError = false;

//            try
//            {
//                if (string.IsNullOrEmpty(Token))
//                {
//                    IsError = true;
//                    Message = "Invalid password reset token.";
//                    return;
//                }

//                var user = await AuthService.GetUserByEmailAsync(UserResetPassword.Email);
//                if (user == null)
//                {
//                    IsError = true;
//                    Message = "Invalid email address.";
//                    return;
//                }

//                var result = await AuthService.ResetPasswordAsync(user.Email, Token, UserResetPassword.Password);
//                if (result.Succeeded)
//                {
//                    Message = "Password has been reset successfully. You can now login with your new password.";
//                    await Task.Delay(2000);
//                    NavigationManager.NavigateTo("/login");
//                }
//                else
//                {
//                    IsError = true;
//                    Message = "Failed to reset password. Please try again.";
//                }
//            }
//            catch (Exception)
//            {
//                IsError = true;
//                Message = "An error occurred. Please try again.";
//            }
//            finally
//            {
//                IsLoading = false;
//            }
//        }

//        public class ResetPasswordModel
//        {
//            [Required(ErrorMessage = "Email is required")]
//            [EmailAddress(ErrorMessage = "Invalid email address")]
//            public string Email { get; set; } = "";

//            [Required(ErrorMessage = "Password is required")]
//            [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
//            public string Password { get; set; } = "";

//            [Required(ErrorMessage = "Confirm password is required")]
//            [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
//            public string ConfirmPassword { get; set; } = "";
//        }
//    }
//} 