using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail, Guid>
    {
        Task<bool> HasConflict(Guid staffId, DateTime start, DateTime end);

    }
}
