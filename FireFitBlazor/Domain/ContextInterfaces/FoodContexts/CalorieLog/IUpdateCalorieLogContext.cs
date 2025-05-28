using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IUpdateCalorieLogContext
    {
        Task<bool> Execute(CalorieLog calorieLog);
    }
