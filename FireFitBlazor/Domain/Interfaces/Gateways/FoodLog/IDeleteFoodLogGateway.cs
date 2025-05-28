using System.Threading.Tasks;


public interface IDeleteFoodLogGateway
{
    Task<bool> DeleteAsync(Guid id);
}
