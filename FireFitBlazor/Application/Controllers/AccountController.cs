﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using static FireFitBlazor.Application.Login;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using System;

namespace FireFitBlazor.Application.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");
            var isMale = dto.Gender == Gender.Male ? true : false;
            var initialUser = Domain.Models.User.Create(
                 Guid.NewGuid().ToString(),
                 dto.Email,
                 BCrypt.Net.BCrypt.HashPassword(dto.Password),
                 dto.Name,
                 dto.Age,
                 isMale,
                 dto.Height,
                 dto.StartingWeight,
                 dto.TargetWeight,
                 dto.WeightGoal,
                 dto.ActivityLevel,
                 dto.DietaryPreferences,
                 dto.WorkoutTypes,
                 null,
                 dto.FitnessExperience
            );

            _context.Users.Add(initialUser);
          
            var initialUserProgress = UserProgress.Create(initialUser.UserId, dto.CurrentWeight, dto.CurrentWeight);

            _context.UserProgress.Add(initialUserProgress);

            var initialUserPreferences = UserPreferences.Create(initialUser.UserId, dto.DietaryPreferences);

            _context.UserPreferences.Add(initialUserPreferences);

            await _context.SaveChangesAsync();

            return Ok("Registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            // Create simple session cookie
            HttpContext.Response.Cookies.Append("userId", user.UserId, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok("Login successful.");
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = HttpContext.Request.Cookies["userId"];
            if (userId == null)
                return Unauthorized();

            var user = _context.Users.Include(i => i.CalorieLogs)
            .FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("userId");
            return Ok("Logged out.");
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}