using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways
{
    public interface IUserPreferencesGateway
    {
        Task<UserPreferences> GetByUserIdAsync(string userId);
        Task<bool> AddAsync(UserPreferences preferences);
        Task<bool> UpdateAsync(UserPreferences preferences);
    }
} 