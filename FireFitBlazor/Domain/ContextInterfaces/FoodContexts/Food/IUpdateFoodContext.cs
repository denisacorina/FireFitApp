using System.Threading.Tasks;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Domain.Models;

    public interface IUpdateFoodContext
    {
        Task<bool> Execute(FoodItem food);
    }
