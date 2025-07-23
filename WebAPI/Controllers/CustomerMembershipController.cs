using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerMembershipController : Controller
    {
        private readonly ICustomerMembershipService _service;

        public CustomerMembershipController(ICustomerMembershipService service)
        {
            _service = service;
        }

        [HttpPost("{customerId}/memberships/{membershipId}")]
        public async Task<IActionResult> AddMembershipToCustomer(Guid customerId, Guid membershipId)
        {
            var result = await _service.CreateMembershipOrderAsync(customerId, membershipId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{customerMembershipId}/memberships/{newMembershipId}")]
        public async Task<IActionResult> UpdateMembership(Guid customerMembershipId, Guid newMembershipId)
        {
            var result = await _service.UpdateCustomerMembershipAsync(customerMembershipId, newMembershipId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPatch("{customerMembershipId}/end")]
        public async Task<IActionResult> EndMembership(Guid customerMembershipId)
        {
            var result = await _service.EndCustomerMembershipAsync(customerMembershipId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetMembershipsByCustomer(Guid customerId)
        {
            var result = await _service.GetMembershipsByCustomerAsync(customerId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("by-customer/best{customerId}")]
        public async Task<IActionResult> GetBestMembershipsByCustomer(Guid customerId)
        {
            var result = await _service.GetBestActiveMembershipByCustomerAsync(customerId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
