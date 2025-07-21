using DTOs.ServiceDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }
        // POST: api/services
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddServiceRequestDTO request)
        {
            var result = await _serviceService.CreateServiceAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        // GET: api/services/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _serviceService.GetServiceByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        //[HttpGet("service/{serviceId}")]
        //public async Task<IActionResult> GetRatingsByServiceId(Guid serviceId)
        //{
        //    var result = await _serviceService.GetRatingsByServiceIdAsync(serviceId);
        //    if (!result.IsSuccess)
        //        return BadRequest(result);
        //    return Ok(result);
        //}


        // GET: api/services
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _serviceService.GetAllServicesAsync();
            if (!result.IsSuccess)
                return BadRequest(result); // Trường hợp lỗi không rõ
            return Ok(result);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetServicesByFilter([FromBody] ServiceFilterDTO filter)
        {
            var result = await _serviceService.GetServicesByFilterAsync(filter);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }


        // PUT: api/services
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceRequest request)
        {
            var result = await _serviceService.UpdateServiceById(request);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        // DELETE: api/services/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _serviceService.SoftDeleteServiceById(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
