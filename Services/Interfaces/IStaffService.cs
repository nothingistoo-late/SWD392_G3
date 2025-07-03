using DTOs.StaffDTO.Request;
using DTOs.StaffDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IStaffService
    {
        Task<ApiResult<StaffRespondDTO>> CreateStaffAsync(CreateStaffRequestDTO dto);
        Task<ApiResult<StaffRespondDTO>> UpdateStaffAsync(Guid staffId, UpdateStaffRequestDTO dto);
        Task<ApiResult<StaffRespondDTO>> SoftDeleteStaffAsync(Guid staffId);
        Task<ApiResult<List<StaffRespondDTO>>> GetAllStaffAsync();
        Task<ApiResult<BulkStaffDeleteResultDTO>> SoftDeleteManyStaffAsync(List<Guid> staffIds);
        Task<ApiResult<List<StaffRespondDTO>>> GetStaffByFilterAsync(StaffFilterDTO filter);
        Task<ApiResult<StaffRespondDTO>> GetStaffByIdAsync(Guid staffId);

    }
}
