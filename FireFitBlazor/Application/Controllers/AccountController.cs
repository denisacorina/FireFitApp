using System;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;

namespace FireFitBlazor.Application.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public CustomAuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _authService.RegisterAsync(dto);
                if (user is null) return BadRequest("Registration failed.");
                await TrySendWelcomeEmailAsync(dto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user is null)
                return Unauthorized("Invalid email or password.");

            return Ok(user);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user is null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok("Logged out.");
        }

        private async Task TrySendWelcomeEmailAsync(RegisterDto dto)
        {
            try
            {
                var welcomeEmailBody = $@"
                <html>
                <body>
                    <h2>Welcome to FireFit, {dto.Name}!</h2>
                    <p>Thank you for joining FireFit. Your fitness journey starts now!</p>
                    <p>Here's what you can do next:</p>
                    <ul>
                        <li>Set up your fitness goals</li>
                        <li>Track your daily nutrition</li>
                        <li>Monitor your progress</li>
                        <li>Get personalized recommendations</li>
                    </ul>
                    <p>Start your journey: <a href='https://yourdomain.com/login'>Login to FireFit</a></p>
                    <p>Best regards,<br>The FireFit Team</p>
                </body>
                </html>";

                await _emailService.SendAsync(dto.Email, "Welcome to FireFit!", welcomeEmailBody, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }
        }
    }
}

