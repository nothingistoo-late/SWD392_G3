using DTOs.MemberShip.Request;
using DTOs.MemberShip.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMembershipService
    {
        Task<ApiResult<List<MembershipResponse>>> GetAllAsync();
        Task<ApiResult<MembershipResponse>> GetByIdAsync(Guid id);
        Task<ApiResult<List<MembershipResponse>>> CreateRangeAsync(List<CreateMembershipRequest> dtos);
        Task<ApiResult<MembershipResponse>> UpdateAsync(Guid id, CreateMembershipRequest dto);
        Task<ApiResult<List<MembershipDeleteResultDTO>>> DeleteAsync(List<Guid> ids);
    }
}
