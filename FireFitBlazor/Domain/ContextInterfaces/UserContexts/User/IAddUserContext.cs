using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IAddUserContext
    {
        Task<bool> Execute(User user);
    }
