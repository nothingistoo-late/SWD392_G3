﻿using DTOs.OrderDTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order, Guid> 
    {
        Task<List<Order>> GetAllWithCustomerAndServiceAsync();
        Task<Order?> GetByIdWithDetailsAsync(Guid id);
        Task<Order?> GetFullOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetAllWithCustomerIdAndServiceAsync(Guid customerid);

    }
}
