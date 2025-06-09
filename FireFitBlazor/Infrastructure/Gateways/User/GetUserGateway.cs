using System.Threading.Tasks;
using FireFitBlazor.Components;
using FireFitBlazor.Components.Pages;
using FireFitBlazor.Domain.Interfaces.Gateways.User;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class GetUserGateway : IGetUserGateway
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserGateway(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetByIdAsync(string id)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(i => i.CalorieLogs)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.Include(i => i.DietaryPreferences)
            .Include(i => i.WorkoutPreferences)
            .Include(i => i.CalorieLogs)
            .Include(i => i.WorkoutTypes)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}

