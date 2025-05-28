using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IUserGateway
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
        Task<bool> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
    }
