using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;



public interface IUpdateGoalContext
{
    Task<bool> Execute(Goal goal);
}
