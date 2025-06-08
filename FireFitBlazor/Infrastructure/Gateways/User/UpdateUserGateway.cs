using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UpdateUserGateway : IUpdateUserGateway
{
    private readonly ApplicationDbContext _context;

    public UpdateUserGateway(ApplicationDbContext context)
    {
        _context = context;
    }

     public async Task<User> GetUserById(string userId)
    {
        return await _context.Users
            .Include(u => u.WorkoutPreferences)
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new InvalidOperationException("User not found.");
    }

    public async Task UpdateAsync(User updatedUser)
    {
        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updatedUser.UserId);
            if (existingUser == null)
                throw new InvalidOperationException("User not found.");

            _context.Entry(existingUser).CurrentValues.SetValues(updatedUser);

            await _context.SaveChangesAsync();

        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}