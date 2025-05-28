using System.Threading.Tasks;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Models;

    public interface IGetFoodContext
    {
        Task<FoodItem> Execute(int id);
        Task<FoodItem> ExecuteByName(string name);
    }
