using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IUpdateUserProgressGateway
    {
        Task<bool> UpdateAsync(UserProgress userProgress);
    }
