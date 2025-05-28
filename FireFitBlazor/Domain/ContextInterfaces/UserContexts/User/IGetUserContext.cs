using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetUserContext
    {
        Task<User> Execute(int id);
        Task<User> ExecuteByEmail(string email);
    }