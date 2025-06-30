using Microsoft.EntityFrameworkCore;
using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class OrderRepository : GenericRepository<Order, Guid>, IOrderRepository
    {
        public OrderRepository(SWD392_G3DBcontext context) : base(context)
        {
        }
        public async Task<List<Order>> GetAllWithCustomerAndServiceAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        // Trong OrderRepository
        public async Task<Order?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetFullOrderByIdAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Staff)
                        .ThenInclude(staff => staff.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }


    }
}
