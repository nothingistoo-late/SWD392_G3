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
        Task<ApiResult<List<MembershipResponse?>>> GetByIdAsync(Guid id);
        Task<ApiResult<List<MembershipResponse>>> CreateAsync(CreateMembershipRequest dto);
        Task<ApiResult<List<MembershipResponse?>>> UpdateAsync(Guid id, CreateMembershipRequest dto);
        Task<ApiResult<List<bool>>> DeleteAsync(Guid id);
    }
}
