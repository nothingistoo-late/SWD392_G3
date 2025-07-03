using DTOs.StaffDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Tạo nhân viên mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequestDTO dto)
        {
            var result = await _staffService.CreateStaffAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// </summary>
        [HttpPut("{staffId}")]
        public async Task<IActionResult> UpdateStaff(Guid staffId, [FromBody] UpdateStaffRequestDTO dto)
        {
            var result = await _staffService.UpdateStaffAsync(staffId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Xoá mềm nhân viên
        /// </summary>
        [HttpDelete("{staffId}")]
        public async Task<IActionResult> SoftDeleteStaff(Guid staffId)
        {
            var result = await _staffService.SoftDeleteStaffAsync(staffId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ nhân viên
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _staffService.GetAllStaffAsync();
            return Ok(result);
        }

        [HttpDelete("soft-delete-many")]
        public async Task<IActionResult> SoftDeleteManyStaff([FromBody] List<Guid> staffIds)
        {
            var result = await _staffService.SoftDeleteManyStaffAsync(staffIds);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetStaffByFilter([FromBody] StaffFilterDTO filter)
        {
            var result = await _staffService.GetStaffByFilterAsync(filter);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }


    }
}
