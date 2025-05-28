using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IAddCalorieLogContext
    {
        Task<bool> Execute(CalorieLog calorieLog);
    }
