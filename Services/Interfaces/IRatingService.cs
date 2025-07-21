using DTOs.RatingDTO.Request;
using DTOs.RatingDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
        public interface IRatingService
        {
            Task<ApiResult<List<ServiceRatingResponseDTO>>> GetRatingsByServiceIdAsync(Guid serviceId);
            Task<ApiResult<bool>> CreateRatingAsync(CreateRatingRequestDTO dto);
            Task<ApiResult<bool>> UpdateRatingAsync(Guid id, UpdateRatingRequestDTO dto);
            Task<ApiResult<bool>> DeleteRatingAsync(Guid id);
            Task<ApiResult<ServiceRatingResponseDTO>> GetRatingByIdAsync(Guid id);
        }
}
