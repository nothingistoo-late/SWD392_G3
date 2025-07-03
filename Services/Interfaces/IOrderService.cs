using DTOs.OrderDTO.Request;
using DTOs.OrderDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResult<OrderRespondDTO>> CreateOrderAsync(CreateOrderRequestDTO dto);
        Task<ApiResult<OrderRespondDTO>> GetOrderByIdAsync(Guid Id);
        Task<ApiResult<List<OrderRespondDTO>>> GetAllOrdersAsync();
        Task<ApiResult<OrderRespondDTO>> UpdateOrderAsync(UpdateOrderRequestDTO dto);
        Task<ApiResult<bool>> MarkWholeOrderCompletedAsync(Guid orderId);
        Task<ApiResult<bool>> CancelWholeOrderAsync(Guid orderId);

        Task<ApiResult<OrderRespondDTO>> SoftDeleteOrderById(Guid orderId);
        Task<List<OrderRespondDTO>> GetOrdersByStaffIdAsync(Guid staffId);
    }
}
