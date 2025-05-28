using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Interfaces.Gateways.Ingredient;
using FireFitBlazor.Infrastructure.Data;
using FireFitBlazor.Infrastructure.Gateways;


public class GetListIngredientGateway : BaseGateway<Ingredient>, IGetListIngredientGateway
{
    public GetListIngredientGateway(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        return await _dbSet
            .Include(i => i.Nutrition)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }
}
