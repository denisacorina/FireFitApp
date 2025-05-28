using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


public interface IUpdateGoalGateway
{
    Task<bool> UpdateAsync(Goal goal);
}
