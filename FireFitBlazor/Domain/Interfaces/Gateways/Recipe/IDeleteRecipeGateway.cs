using System.Threading.Tasks;

namespace FireFitBlazor.Domain.Interfaces.Gateways.Recipe
{
    public interface IDeleteRecipeGateway
    {
        Task<bool> DeleteAsync(int id);
    }
} 