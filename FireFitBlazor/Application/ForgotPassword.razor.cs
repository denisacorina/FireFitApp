//using System.ComponentModel.DataAnnotations;
//using FireFitBlazor.Domain.Services;
//using Microsoft.AspNetCore.Components;

//namespace FireFitBlazor.Application
//{
//    public partial class ForgotPassword
//    {
//        [Inject]
//        private IAuthService AuthService { get; set; } = default!;

//        [Inject]
//        private NavigationManager NavigationManager { get; set; } = default!;

//        public ForgotPasswordModel UserForgotPassword = new();
//        private string Message = "";
//        public bool IsError = false;
//        public bool IsLoading = false;

//        private async Task HandleValidSubmit()
//        {
//            IsLoading = true;
//            Message = "";
//            IsError = false;

//            try
//            {
//                var user = await AuthService.GetUserByEmailAsync(UserForgotPassword.Email);
//                if (user == null)
//                {
//                    // Don't reveal that the user does not exist
//                    Message = "If your email is registered, you will receive a password reset link.";
//                    return;
//                }

//                var token = await AuthService.GeneratePasswordResetTokenAsync(user.Email);
//                // TODO: Send email with reset link
//                // For now, just show a success message
//                Message = "If your email is registered, you will receive a password reset link.";
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

//        public class ForgotPasswordModel
//        {
//            [Required(ErrorMessage = "Email is required")]
//            [EmailAddress(ErrorMessage = "Invalid email address")]
//            public string Email { get; set; } = "";
//        }
//    }
//} 