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
                .Where(o => !o.IsDeleted)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Service)
                .Where(o => !o.Customer.IsDeleted && !o.Customer.User.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // Trong OrderRepository
        public async Task<Order?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted && o.Id == id)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Service)
                .FirstOrDefaultAsync(o => !o.Customer.IsDeleted && !o.Customer.User.IsDeleted);
        }


        public async Task<Order?> GetFullOrderByIdAsync(Guid orderId)
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && o.Id == orderId)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Service)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Staff)
                        .ThenInclude(staff => staff.User)
                .FirstOrDefaultAsync(o =>
                    o.OrderDetails.All(od =>
                        !od.Service.IsDeleted &&
                        !od.Staff.IsDeleted &&
                        !od.Staff.User.IsDeleted));
        }



    }
}
