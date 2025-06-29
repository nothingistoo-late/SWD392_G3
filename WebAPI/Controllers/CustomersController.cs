using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Lấy tất cả khách hàng (chưa bị xóa mềm)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới khách hàng (User + Customer)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequestDTO dto)
        {
            var result = await _customerService.CreateCustomerAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy thông tin khách hàng theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Xóa mềm khách hàng
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCustomer(Guid id)
        {
            var result = await _customerService.SoftDeleteCustomerById(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
