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
        public async Task<IActionResult> MarkCompleted(Guid orderDetailId, [FromBody] string? note)
        {
            var result = await _orderDetailService.MarkOrderDetailCompletedAsync(orderDetailId, note);
            return Ok(result);
        }
        [HttpPatch("{orderDetailId}/cancel")]
        public async Task<IActionResult> Cancel(Guid orderDetailId, [FromBody] string? note)
        {
            var result = await _orderDetailService.CancelOrderDetailAsync(orderDetailId, note);
            return Ok(result);
        }

        [HttpPatch("{orderDetailId}/reschedule")]
        public async Task<IActionResult> Reschedule(Guid orderDetailId, [FromBody] DateTime newTime)
        {
            var result = await _orderDetailService.RescheduleOrderDetailAsync(orderDetailId, newTime);
            return Ok(result);
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderDetailStatusAndNoteRequestDTO dto)
        {
            var result = await _orderDetailService.UpdateOrderDetailStatusAsync(dto);
            return Ok(result);
        }

    }
}
