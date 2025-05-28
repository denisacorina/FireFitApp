using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IUpdateFoodLogContext
    {
        Task<bool> Execute(FoodLog foodLog);
    }
