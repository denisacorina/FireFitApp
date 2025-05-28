using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.User;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts.User;


public class GetUserContext : IGetUserContext
{
    private readonly IGetUserGateway _getUserGateway;

    public GetUserContext(IGetUserGateway getUserGateway)
    {
        _getUserGateway = getUserGateway ?? throw new ArgumentNullException(nameof(getUserGateway));
    }

    public async Task<User> Execute(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(Messages.Error_InvalidId);
        }

        return await _getUserGateway.GetByIdAsync(id);
    }

    public async Task<User> ExecuteByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(Messages.Error_EmptyEmail);
        }

        return await _getUserGateway.GetByEmailAsync(email);
    }
}
