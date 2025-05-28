using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IGetListIngredientGateway
{
    Task<IEnumerable<Ingredient>> GetAllAsync();
}
