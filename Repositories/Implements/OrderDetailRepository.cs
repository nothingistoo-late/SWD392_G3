using Microsoft.EntityFrameworkCore;
using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, Guid>, IOrderDetailRepository
    {
        public OrderDetailRepository(SWD392_G3DBcontext context) : base(context)
        {
        }

        public async Task<bool> HasConflict(Guid staffId, DateTime start, DateTime end)
        {
            return await _context.OrderDetails
                        .Include(od => od.Service)
                        .Where(od => od.StaffId == staffId)
                        .AnyAsync(od =>
                            start < od.ScheduleTime.AddMinutes(od.Service.Duration) &&
                            end > od.ScheduleTime);
        }
    }
}
