using AutoMapper;
using DTOs.OrderDTO.Request;
using DTOs.OrderDTO.Respond;
using Repositories.Implements;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class OrderService : BaseService<Order, Guid>, IOrderService
    {
        private readonly IMapper _mapper; 
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository, IGenericRepository<Order, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<ApiResult<OrderRespondDTO>> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    OrderDate = request.OrderDate,
                    CreatedAt = _currentTime.GetVietnamTime(),
                    CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty,
                    OrderDetails = request.ServiceIds.Select(sid => new OrderDetail
                    {
                        ServiceId = sid,
                        CreatedAt = _currentTime.GetVietnamTime(),
                        CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty
                    }).ToList()
                };

                await _repository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<OrderRespondDTO>(order);
                return ApiResult<OrderRespondDTO>.Success(dto, "Tạo đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi tạo đơn hàng: " + ex.Message));
            }
        }


        public async Task<ApiResult<List<OrderRespondDTO>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllWithCustomerAndServiceAsync();
                var result = _mapper.Map<List<OrderRespondDTO>>(orders);
                return ApiResult<List<OrderRespondDTO>>.Success(result, "Lấy tất cả đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrderRespondDTO>>.Failure(new Exception("Lỗi khi lấy danh sách đơn hàng: " + ex.Message));
            }
        }

        public async Task<ApiResult<OrderRespondDTO>> GetOrderByIdAsync(Guid Id)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithDetailsAsync(Id);

                if (order == null)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Không tìm thấy đơn hàng!"));

                var dto = _mapper.Map<OrderRespondDTO>(order);
                return ApiResult<OrderRespondDTO>.Success(dto, "Lấy đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi lấy đơn hàng: " + ex.Message));
            }
        }


        public async Task<ApiResult<OrderRespondDTO>> SoftDeleteOrderById(Guid orderId)
        {
            try
            {
                var order = await _repository.GetByIdAsync(orderId);

                if (order == null)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Không tìm thấy đơn hàng để xóa!"));

                order.IsDeleted = true;
                order.DeletedAt = _currentTime.GetVietnamTime();
                order.DeletedBy = _currentUserService.GetUserId() ?? Guid.Empty;

                await UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<OrderRespondDTO>(order);
                return ApiResult<OrderRespondDTO>.Success(result, "Xóa mềm đơn hàng thành công!");
            }
            catch (Exception e)
            {
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi xóa mềm đơn hàng: " + e.Message));
            }
        }

        public async Task<ApiResult<OrderRespondDTO>> UpdateOrderById(UpdateOrderRequest request)
        {
            try
            {
                var order = await _repository.GetByIdAsync(request.OrderId, o => o.OrderDetails);

                if (order == null)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Không tìm thấy đơn hàng!"));

                // ✅ Chỉ update field nếu có giá trị
                if (request.OrderDate.HasValue)
                    order.OrderDate = request.OrderDate.Value;

                // ✅ Nếu có ServiceIds mới → cập nhật lại OrderDetails
                if (request.ServiceIds is { Count: > 0 })
                {
                    order.OrderDetails.Clear();
                    foreach (var sid in request.ServiceIds)
                    {
                        order.OrderDetails.Add(new OrderDetail
                        {
                            ServiceId = sid,
                            CreatedAt = _currentTime.GetVietnamTime(),
                            CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty
                        });
                    }
                }

                order.UpdatedAt = _currentTime.GetVietnamTime();
                order.UpdatedBy = _currentUserService.GetUserId() ?? Guid.Empty;

                await _repository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<OrderRespondDTO>(order);
                return ApiResult<OrderRespondDTO>.Success(dto, "Cập nhật đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi cập nhật đơn hàng: " + ex.Message));
            }
        }
    }
}
