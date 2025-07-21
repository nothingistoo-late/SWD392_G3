using DTOs.CustomerMembership.Respond;
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
    }
}
