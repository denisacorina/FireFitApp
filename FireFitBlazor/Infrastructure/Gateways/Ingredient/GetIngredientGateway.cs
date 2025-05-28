using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;

public class GetIngredientGateway : BaseGateway<Ingredient>, IGetIngredientGateway
{
    public GetIngredientGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ingredient> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Nutrition)
            .FirstOrDefaultAsync(i => i.IngredientId == id);
    }

    public async Task<Ingredient> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(i => i.Nutrition)
            .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
    }
}
