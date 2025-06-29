using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.WorkSeeds.Interfaces
{
    public interface IUnitOfWork : IGenericUnitOfWork
    {
        // Repository cho người dùng
        IUserRepository UserRepository { get; }

    }
}
