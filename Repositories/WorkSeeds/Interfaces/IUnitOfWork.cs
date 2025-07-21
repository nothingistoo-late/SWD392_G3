using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.WorkSeeds.Interfaces
{
    public interface IUnitOfWork : IGenericUnitOfWork
    {
        // Repository cho người dùng
        IUserRepository UserRepository { get; }

        ICustomerRepository CustomerRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderRepository OrderRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IStaffRepository StaffRepository { get; }
        IStaffScheduleRepository StaffScheduleRepository { get; }
        IRatingRepository RatingRepository { get; }
        IMembershipRepository MembershipRepository { get; }

        ICustomerMemberShipRepository CustomerMemberShipRepository { get; }
    }
}
