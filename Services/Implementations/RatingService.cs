using DTOs.RatingDTO.Request;
using DTOs.RatingDTO.Respond;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class RatingService : BaseService<Rating, Guid>, IRatingService
    {
        public RatingService(IGenericRepository<Rating, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
        }

        public async Task<ApiResult<List<ServiceRatingResponseDTO>>> GetRatingsByServiceIdAsync(Guid serviceId)
        {
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);

                if (service == null)
                {
                    return ApiResult<List<ServiceRatingResponseDTO>>.Failure(new Exception("Dịch vụ không tồn tại."));
                }

                var ratings = await _unitOfWork.RatingRepository.GetAllAsync(
                            r => r.OrderDetail.ServiceId == serviceId,
                            null,
                            r => r.OrderDetail,
                            r => r.OrderDetail.Order,
                            r => r.OrderDetail.Order.Customer,
                            r => r.OrderDetail.Order.Customer.User
                        );

                if (ratings == null || !ratings.Any())
                {
                    return ApiResult<List<ServiceRatingResponseDTO>>.Failure(new Exception("Không tìm thấy đánh giá nào cho dịch vụ này."));
                }

                var result = ratings.Select(r => new ServiceRatingResponseDTO
                {
                    Score = r.Score,
                    Comment = r.Comment,
                    CreateDate = r.CreatedAt,
                    CustomerName = r.OrderDetail?.Order?.Customer?.User?.FullName ?? "Không rõ",
                }).ToList();


                return ApiResult<List<ServiceRatingResponseDTO>>.Success(result,"Lấy đánh giá theo id dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResult<List<ServiceRatingResponseDTO>>.Failure(
                    new Exception("Lỗi khi lấy đánh giá dịch vụ: " + ex.Message));
            }
        }

        public async Task<ApiResult<bool>> CreateRatingAsync(CreateRatingRequestDTO dto)
        {
            try
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository
                    .FirstOrDefaultAsync(o => o.OrderDetailId== dto.OrderDetailId); // hoặc od => od.Id (tùy context)
                if (orderDetail == null)
                    return ApiResult<bool>.Failure(new Exception("OrderDetail không tồn tại."));

                var rating = new Rating
                {
                    Id = Guid.NewGuid(),
                    OrderDetailId = dto.OrderDetailId,
                    Score = dto.Score,
                    Comment = dto.Comment,
                    CreatedAt = _currentTime.GetVietnamTime(),
                    CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty,
                };

                await _unitOfWork.RatingRepository.AddAsync(rating);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<bool>.Success(true,"Tạo đánh giá thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi tạo đánh giá: " + ex.Message));
            }
        }

        public async Task<ApiResult<bool>> UpdateRatingAsync(Guid id, UpdateRatingRequestDTO dto)
        {
            try
            {
                var rating = await _unitOfWork.RatingRepository.GetByIdAsync(id);
                if (rating == null)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy đánh giá."));

                var vietnamNow = _currentTime.GetVietnamTime();
                if ((vietnamNow - rating.CreatedAt).TotalDays > 7)
                {
                    return ApiResult<bool>.Failure(
                        new Exception("Không thể cập nhật đánh giá sau 7 ngày kể từ khi tạo."));
                }

                rating.Score = dto.Score;
                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    rating.Comment = dto.Comment;
                }
                rating.UpdatedAt = vietnamNow;

                await  _unitOfWork.RatingRepository.UpdateAsync(rating);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<bool>.Success(true,"Cập nhật đánh gía thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi cập nhật đánh giá: " + ex.Message));
            }
        }

        public async Task<ApiResult<bool>> DeleteRatingAsync(Guid id)
        {
            try
            {
                var rating = await _unitOfWork.RatingRepository.GetByIdAsync(id);
                if (rating == null)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy đánh giá."));

                await _unitOfWork.RatingRepository.SoftDeleteAsync(rating.Id);
                rating.DeletedBy = _currentUserService.GetUserId() ?? Guid.Empty;
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<bool>.Success(true,"xóa mềm đánh giá thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi xoá mềm đánh giá: " + ex.Message));
            }
        }

        public async Task<ApiResult<ServiceRatingResponseDTO>> GetRatingByIdAsync(Guid id)
        {
            try
            {
                var ratingList = await _unitOfWork.RatingRepository.GetAllAsync(
                         predicate: r => r.Id == id,
                         includes: new Expression<Func<Rating, object>>[]
                         {
                            r => r.OrderDetail,
                            r => r.OrderDetail.Order,
                            r => r.OrderDetail.Order.Customer,
                            r => r.OrderDetail.Order.Customer.User
                         }
                     );


                var r = ratingList.FirstOrDefault();
                if (r == null)
                    return ApiResult<ServiceRatingResponseDTO>.Failure(new Exception("Không tìm thấy đánh giá."));

                var result = new ServiceRatingResponseDTO
                {
                    Score = r.Score,
                    Comment = r.Comment ?? "Không có nhận xét!",
                    CreateDate = r.CreatedAt,
                    CustomerName = r.OrderDetail?.Order?.Customer?.User?.FullName ?? "Không rõ"
                };

                return ApiResult<ServiceRatingResponseDTO>.Success(result, "Lấy đánh giá theo id thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<ServiceRatingResponseDTO>.Failure(
                    new Exception("Lỗi khi lấy chi tiết đánh giá: " + ex.Message));
            }
        }

    }
}
