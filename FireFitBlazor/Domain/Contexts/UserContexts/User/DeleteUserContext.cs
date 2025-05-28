using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.User;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts.User;

namespace FireFitBlazor.Domain.Contexts.UserContexts.User
{
    public class DeleteUserContext : IDeleteUserContext
    {
        private readonly IDeleteUserGateway _deleteUserGateway;

        public DeleteUserContext(IDeleteUserGateway deleteUserGateway)
        {
            _deleteUserGateway = deleteUserGateway ?? throw new ArgumentNullException(nameof(deleteUserGateway));
        }

        public async Task<bool> Execute(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(Messages.Error_InvalidId);
            }

            return await _deleteUserGateway.DeleteAsync(id);
        }
    }
} 