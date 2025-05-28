using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetListIngredientContext
    {
        Task<IEnumerable<Ingredient>> Execute();
    }
