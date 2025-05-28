using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

public interface IAddGoalContext
{
    Task<bool> Execute(Goal goal);
}
