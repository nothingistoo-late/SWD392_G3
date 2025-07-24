using DTOs.OrderDetailDTO.Request;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class OrderDetailService : BaseService<OrderDetail, Guid>, IOrderDetailService
    {
        public OrderDetailService(IGenericRepository<OrderDetail, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
        }

        public async Task<ApiResult<bool>> MarkOrderDetailCompletedAsync(Guid orderDetailId, string? note)
        {
            var dto = new UpdateOrderDetailStatusAndNoteRequestDTO
            {
                OrderDetailId = orderDetailId,
                NewStatus = OrderDetailStatus.Completed,
                Note = note
            };

            return await UpdateOrderDetailStatusAsync(dto);
        }


        public async Task<ApiResult<bool>> CancelOrderDetailAsync(Guid orderDetailId, string? note)
        {
            var dto = new UpdateOrderDetailStatusAndNoteRequestDTO
            {
                OrderDetailId = orderDetailId,
                NewStatus = OrderDetailStatus.Cancelled,
                Note = note
            };

            return await UpdateOrderDetailStatusAsync(dto);
        }


        public async Task<ApiResult<bool>> UpdateOrderDetailStatusAsync(UpdateOrderDetailStatusAndNoteRequestDTO dto)
        {
            try
            {
                var detail = await _unitOfWork.OrderDetailRepository
                    .FirstOrDefaultAsync(o => o.OrderDetailId == dto.OrderDetailId);

                if (detail == null || detail.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy chi tiết đơn hàng hoặc đã bị xoá."));

                // Cập nhật trạng thái + ghi chú
                detail.Status = dto.NewStatus;
                detail.Note = dto.Note;

                // Lấy toàn bộ các order detail thuộc cùng order
                var allDetails = await _unitOfWork.OrderDetailRepository
                    .GetAllAsync(o => o.OrderId == detail.OrderId && !o.IsDeleted);

                // Tìm status nhỏ nhất
                var minStatus = allDetails.Min(d => (int)d.Status);
                // Cập nhật trạng thái order cha
                var order = await _unitOfWork.OrderRepository
                    .FirstOrDefaultAsync(o => o.Id == detail.OrderId);

                if (order != null)
                {
                    order.Status = (OrderStatus)minStatus;
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, $"Cập nhật trạng thái thành công: {dto.NewStatus}");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi cập nhật trạng thái: " + ex.Message));
            }
        }



        public async Task<ApiResult<bool>> RescheduleOrderDetailAsync(Guid orderDetailId, DateTime newTime)
        {
            try
            {
                var detail = await _unitOfWork.OrderDetailRepository.FirstOrDefaultAsync(o=> o.OrderDetailId== orderDetailId);
                if (detail == null || detail.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy chi tiết đơn hàng hoặc đã bị xoá."));

                detail.ScheduleTime = newTime;
                detail.Status = OrderDetailStatus.Pending; // Đổi lịch → xem như đã xác nhận lại
                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, "Đổi lịch thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi đổi lịch: " + ex.Message));
            }
        }
    }
}
