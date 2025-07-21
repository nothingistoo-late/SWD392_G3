using DTOs.MemberShip.Request;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        // GET: api/Membership
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _membershipService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _membershipService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRangeAsync([FromBody] List<CreateMembershipRequest> dtos)
        {
            var result = await _membershipService.CreateRangeAsync(dtos);
            if (result.IsSuccess)
                return Ok(result); 
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateMembershipRequest dto)
        {
            var result = await _membershipService.UpdateAsync(id, dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromBody] List<Guid> ids)
        {
            var result = await _membershipService.DeleteAsync(ids);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
