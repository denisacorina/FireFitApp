using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Models;


    public interface IGetListFoodContext
    {
        Task<IEnumerable<FoodItem>> Execute();
        Task<IEnumerable<FoodItem>> ExecuteByCategory(string category);
    }
