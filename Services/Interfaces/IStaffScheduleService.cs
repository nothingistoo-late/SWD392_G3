using DTOs.StaffScheduleDTO.Request;
using DTOs.StaffScheduleDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IStaffScheduleService
    {
        Task<ApiResult<List<StaffScheduleRespondDTO>>> GetSchedulesByStaffIdAsync(Guid staffId);
        Task<ApiResult<StaffScheduleRespondDTO>> CreateScheduleAsync(CreateStaffScheduleRequestDTO dto);
        Task<ApiResult<StaffScheduleRespondDTO>> UpdateScheduleAsync(Guid scheduleId, UpdateStaffScheduleRequestDTO dto);
        Task<ApiResult<bool>> SoftDeleteScheduleAsync(Guid scheduleId);
        Task<ApiResult<List<StaffScheduleRespondDTO>>> FilterSchedulesAsync(StaffScheduleFilterDTO filter);
        Task<ApiResult<List<AvailableSlotDTO>>> GetAvailableSlotsAsync(StaffAvailableSlotsRequestDTO request);
        Task<ApiResult<List<AvailableStaffDTO>>> GetAvailableStaffInTimeRangeAsync(AvailableStaffRequestDTO dto);
    }
}
