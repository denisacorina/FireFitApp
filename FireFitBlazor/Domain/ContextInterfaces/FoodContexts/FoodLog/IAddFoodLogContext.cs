using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IAddFoodLogContext
    {
        Task<bool> Execute(FoodLog foodLog);
    }