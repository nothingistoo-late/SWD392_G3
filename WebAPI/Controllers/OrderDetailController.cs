using DTOs.OrderDetailDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : Controller
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpPatch("{orderDetailId}/complete")]
        public async Task<IActionResult> MarkCompleted(Guid orderDetailId)
        {
            var result = await _orderDetailService.MarkOrderDetailCompletedAsync(orderDetailId);
            return Ok(result);
        }

        [HttpPatch("{orderDetailId}/cancel")]
        public async Task<IActionResult> Cancel(Guid orderDetailId)
        {
            var result = await _orderDetailService.CancelOrderDetailAsync(orderDetailId);
            return Ok(result);
        }

        [HttpPatch("{orderDetailId}/reschedule")]
        public async Task<IActionResult> Reschedule(Guid orderDetailId, [FromBody] DateTime newTime)
        {
            var result = await _orderDetailService.RescheduleOrderDetailAsync(orderDetailId, newTime);
            return Ok(result);
        }

        [HttpPatch("{orderDetailId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid orderDetailId, [FromBody] UpdateOrderDetailStatusRequestDTO dto)
        {
            var result = await _orderDetailService.UpdateOrderDetailStatusAsync(orderDetailId, dto.NewStatus);
            return Ok(result);
        }

    }
}
