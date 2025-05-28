using FireFitBlazor.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FireFitBlazor
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CustomAuthStateProvider(IHttpContextAccessor accessor, ApplicationDbContext context)
        {
            _httpContextAccessor = accessor;
            _context = context;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];
            if (string.IsNullOrWhiteSpace(userId)) return Anonymous();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return Anonymous();

            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.Name)
        }, "Custom");

            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }

        private AuthenticationState Anonymous()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

}
