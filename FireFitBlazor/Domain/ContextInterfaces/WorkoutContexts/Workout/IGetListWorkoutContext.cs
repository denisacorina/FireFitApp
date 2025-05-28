using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetListWorkoutContext
    {
        Task<IEnumerable<WorkoutSession>> Execute();
        Task<IEnumerable<WorkoutSession>> ExecuteByType(string type);
    }
