using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IAddGoalGateway
{
    Task<bool> AddAsync(Goal goal);
}
