using DTOs.RatingDTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetRatingsByServiceId(Guid serviceId)
        {
            var result = await _ratingService.GetRatingsByServiceIdAsync(serviceId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingRequestDTO dto)
        {
            var result = await _ratingService.CreateRatingAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(Guid id, [FromBody] UpdateRatingRequestDTO dto)
        {
            var result = await _ratingService.UpdateRatingAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(Guid id)
        {
            var result = await _ratingService.DeleteRatingAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(Guid id)
        {
            var result = await _ratingService.GetRatingByIdAsync(id);
            return Ok(result);
        }
    }

}
