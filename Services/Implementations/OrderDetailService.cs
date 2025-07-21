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

        public async Task<ApiResult<bool>> MarkOrderDetailCompletedAsync(Guid orderDetailId, string note)
        {
            return await UpdateOrderDetailStatusAsync(orderDetailId, OrderDetailStatus.Completed, note);
        }

        public async Task<ApiResult<bool>> CancelOrderDetailAsync(Guid orderDetailId)
        {
            return await UpdateOrderDetailStatusAsync(orderDetailId, OrderDetailStatus.Cancelled);
        }

        public async Task<ApiResult<bool>> UpdateOrderDetailStatusAsync(Guid orderDetailId, OrderDetailStatus newStatus, string? note = null)
        {
            try
            {
                var detail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId);
                if (detail == null || detail.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy chi tiết đơn hàng hoặc đã bị xoá."));

                detail.Status = newStatus;
                detail.Note = note;
                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, $"Cập nhật trạng thái thành công: {newStatus}");
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
                var detail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId);
                if (detail == null || detail.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy chi tiết đơn hàng hoặc đã bị xoá."));

                detail.ScheduleTime = newTime;
                detail.Status = OrderDetailStatus.Confirmed; // Đổi lịch → xem như đã xác nhận lại
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
