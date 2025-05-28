using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetCalorieLogContext
    {
        Task<CalorieLog> Execute(int id);
        Task<CalorieLog> ExecuteByUserAndDate(int userId, DateTime date);
    }
