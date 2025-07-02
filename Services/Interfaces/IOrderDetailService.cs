using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<ApiResult<bool>> MarkOrderDetailCompletedAsync(Guid orderDetailId);
        Task<ApiResult<bool>> CancelOrderDetailAsync(Guid orderDetailId);
        Task<ApiResult<bool>> RescheduleOrderDetailAsync(Guid orderDetailId, DateTime newTime);
        Task<ApiResult<bool>> UpdateOrderDetailStatusAsync(Guid orderDetailId, OrderDetailStatus newStatus);
    }
}
