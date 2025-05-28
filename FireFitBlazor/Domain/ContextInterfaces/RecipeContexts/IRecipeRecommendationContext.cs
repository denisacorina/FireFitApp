using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.ContextInterfaces.RecipeContexts
{
    public interface IRecipeRecommendationContext
    {
        Task<List<Recipe>> Execute(int userId);
    }
} 