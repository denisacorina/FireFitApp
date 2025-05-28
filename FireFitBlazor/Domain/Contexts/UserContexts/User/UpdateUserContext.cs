using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.User;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts.User;

public class UpdateUserContext : IUpdateUserContext
{
    private readonly IUpdateUserGateway _updateUserGateway;

    public UpdateUserContext(IUpdateUserGateway updateUserGateway)
    {
        _updateUserGateway = updateUserGateway ?? throw new ArgumentNullException(nameof(updateUserGateway));
    }

    public async Task<bool> Execute(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), Messages.Error_NullEntity);
        }

        if (user.UserId == string.Empty)
        {
            throw new ArgumentException(Messages.Error_InvalidId);
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException(Messages.Error_EmptyEmail);
        }

        if (string.IsNullOrWhiteSpace(user.Name))
        {
            throw new ArgumentException(Messages.Error_EmptyFirstName);
        }

        return await _updateUserGateway.UpdateAsync(user);
    }
}
