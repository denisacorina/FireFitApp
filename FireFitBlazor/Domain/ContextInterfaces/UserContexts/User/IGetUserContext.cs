using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetUserContext
    {
        Task<User> Execute(string id);
        Task<User> ExecuteByEmail(string email);
    }