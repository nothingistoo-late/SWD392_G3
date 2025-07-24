using DTOs.CustomerMembership.Respond;
using DTOs.OrderDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICustomerMembershipService
    {
        Task<ApiResult<List<CustomerMembershipResponse>>> GetMembershipsByCustomerAsync(Guid customerId);
        Task<ApiResult<CustomerMembershipResponse>> EndCustomerMembershipAsync(Guid cmId);
        Task<ApiResult<CustomerMembershipResponse>> UpdateCustomerMembershipAsync(Guid cmId, Guid newMembershipId);
        Task<ApiResult<CustomerMembershipResponse>> AddMembershipToCustomerAsync(Guid customerId, Guid membershipId);
        Task<ApiResult<CustomerMembershipResponse>> GetBestActiveMembershipByCustomerAsync(Guid customerId);
        Task<ApiResult<CustomerMembershipWithOrderResponse>> CreateMembershipOrderForCustomerAsync(Guid customerId, Guid membershipId, Guid orderId);
        Task<ApiResult<OrderRespondDTO>> CreateMembershipOrderAsync(Guid customerId, Guid membershipId);
    }
}
