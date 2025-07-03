using DTOs.StaffScheduleDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/staff-schedules")]
    public class StaffScheduleController : Controller
    {
        private readonly IStaffScheduleService _staffScheduleService;

        public StaffScheduleController(IStaffScheduleService staffScheduleService)
        {
            _staffScheduleService = staffScheduleService;
        }

        /// <summary>
        /// Lấy tất cả lịch làm việc của 1 nhân viên
        /// </summary>
        [HttpGet("by-staff/{staffId}")]
        public async Task<IActionResult> GetSchedulesByStaffId(Guid staffId)
        {
            var result = await _staffScheduleService.GetSchedulesByStaffIdAsync(staffId);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Tạo lịch làm việc mới cho nhân viên
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateStaffScheduleRequestDTO dto)
        {
            var result = await _staffScheduleService.CreateScheduleAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật lịch làm việc theo ID
        /// </summary>
        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(Guid scheduleId, [FromBody] UpdateStaffScheduleRequestDTO dto)
        {
            var result = await _staffScheduleService.UpdateScheduleAsync(scheduleId, dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Xoá mềm lịch làm việc
        /// </summary>
        [HttpDelete("{scheduleId}/soft-delete")]
        public async Task<IActionResult> SoftDeleteSchedule(Guid scheduleId)
        {
            var result = await _staffScheduleService.SoftDeleteScheduleAsync(scheduleId);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lọc lịch làm việc theo điều kiện
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterSchedules([FromBody] StaffScheduleFilterDTO filter)
        {
            var result = await _staffScheduleService.FilterSchedulesAsync(filter);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromBody] StaffAvailableSlotsRequestDTO request)
        {
            var result = await _staffScheduleService.GetAvailableSlotsAsync(request);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("available-staffs")]
        public async Task<IActionResult> GetAvailableStaffs([FromBody] AvailableStaffRequestDTO dto)
        {
            var result = await _staffScheduleService.GetAvailableStaffInTimeRangeAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStaffSchedules()
        {
            var result = await _staffScheduleService.GetAllStaffSchedulesAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);

        }
    }
}
