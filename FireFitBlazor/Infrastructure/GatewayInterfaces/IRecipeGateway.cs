using FireFitBlazor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireFitBlazor.Infrastructure.GatewayInterfaces
{
    public interface IRecipeGateway
    {
        Task<UserPreferences> GetUserPreferencesByUserId(string userId);
        Task<List<Ingredient>> GetAvailableIngredientsForUser(UserPreferences userPreferences);
        Task SaveRecipe(Recipe recipe);
    }
}
