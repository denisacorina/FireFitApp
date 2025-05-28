using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FireFitBlazor.Application.Services
{
    public interface ICurrentUserService
    {
        Task<User?> GetUserAsync();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _db;
        private User? _cachedUser;

        public CurrentUserService(IHttpContextAccessor accessor, ApplicationDbContext db)
        {
            _accessor = accessor;
            _db = db;
        }

        public async Task<User?> GetUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var userId = _accessor.HttpContext?.Request.Cookies["userId"];
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            _cachedUser = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return _cachedUser;
        }
    }
}
