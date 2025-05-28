using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Ingredient
{
    public interface IDeleteIngredientGateway
    {
        Task<bool> DeleteAsync(Guid id);
    }
} 