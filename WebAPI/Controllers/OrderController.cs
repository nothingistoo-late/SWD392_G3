﻿using DTOs.OrderDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDTO request)
        {
            var result = await _orderService.CreateOrderAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy tất cả đơn hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy đơn hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Cập nhật đơn hàng theo ID
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOrderRequestDTO request)
        {
            var result = await _orderService.UpdateOrderAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xoá mềm đơn hàng theo ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _orderService.SoftDeleteOrderById(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{orderId}/complete")]
        public async Task<IActionResult> MarkOrderCompleted(Guid orderId)
        {
            var result = await _orderService.MarkWholeOrderCompletedAsync(orderId);
            return Ok(result);
        }

        [HttpPatch("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var result = await _orderService.CancelWholeOrderAsync(orderId);
            return Ok(result);
        }

        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetOrdersByStaffId(Guid staffId)
        {
            var result = await _orderService.GetOrdersByStaffIdAsync(staffId);
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAllByCustomerId(Guid customerId)
        {
            var result = await _orderService.GetAllOrdersByCustomerIdAsync(customerId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusRequestDTO newStatus)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
    }
}
