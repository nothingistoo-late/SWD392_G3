using DTOs.OrderDetailDTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<ApiResult<bool>> MarkOrderDetailCompletedAsync(Guid orderDetailId, string? note);
        Task<ApiResult<bool>> CancelOrderDetailAsync(Guid orderDetailId, string? note);
        Task<ApiResult<bool>> RescheduleOrderDetailAsync(Guid orderDetailId, DateTime newTime);
        Task<ApiResult<bool>> UpdateOrderDetailStatusAsync(UpdateOrderDetailStatusAndNoteRequestDTO dto);
    }
}
