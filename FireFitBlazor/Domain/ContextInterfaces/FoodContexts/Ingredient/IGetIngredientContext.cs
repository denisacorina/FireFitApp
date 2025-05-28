using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetIngredientContext
    {
        Task<Ingredient> Execute(Guid id);
        Task<Ingredient> ExecuteByName(string name);
    }