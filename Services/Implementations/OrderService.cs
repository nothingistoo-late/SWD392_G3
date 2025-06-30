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
        private readonly IServiceRepository _serviceRepository;
        private readonly IStaffScheduleRepository _staffCheduleRepository;
        private readonly IOrderDetailRepository _orderDetailRepo;

        public OrderService(IOrderDetailRepository orderDetailRepository, IStaffScheduleRepository staffCheduleRepository, IServiceRepository serviceRepository, IMapper mapper, IOrderRepository orderRepository, IGenericRepository<Order, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _serviceRepository = serviceRepository;
            _staffCheduleRepository = staffCheduleRepository;
            _orderDetailRepo = orderDetailRepository;
        }

        public async Task<ApiResult<OrderRespondDTO>> CreateOrderAsync(CreateOrderRequestDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await _unitOfWork.UserRepository.GetUserDetailsByIdAsync(dto.CustomerId);
                if (customer == null || customer.IsDeleted)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception($"Khách hàng với ID {dto.CustomerId} không tồn tại."));

                if (dto.OrderDate < _currentTime.GetVietnamTime())
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Ngày đặt hàng không được trong quá khứ."));

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    OrderDetails = new List<OrderDetail>()
                };

                double total = 0;

                foreach (var item in dto.Services)
                {
                    // 1. Get service info
                    var service = await _serviceRepository.GetByIdAsync(item.ServiceId);
                    if (service == null || service.IsDeleted)
                        return ApiResult<OrderRespondDTO>.Failure(new Exception($"Dịch vụ với ID {item.ServiceId} không tồn tại."));

                    var start = item.ScheduledTime;
                    var end = start.AddMinutes(service.Duration);

                    // 2. Find available staff
                    var staff = await FindAvailableStaffAsync(service.Id, start, end);
                    if (staff == null)
                        return ApiResult<OrderRespondDTO>.Failure(new Exception($"Không tìm thấy nhân viên rảnh cho dịch vụ lúc {start:HH:mm dd/MM}"));

                    // 3. Create OrderDetail (✅ nhớ gán ScheduleTime)
                    order.OrderDetails.Add(new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        ServiceId = service.Id,
                        ScheduleTime = start,
                        StaffId = staff.Id,
                        OrderId = order.Id
                    });

                    total += service.Price;
                }

                order.TotalPrice = total;

                await CreateAsync(order);
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Reload lại Order kèm navigation
                var fullOrder = await _unitOfWork.OrderRepository.GetFullOrderByIdAsync(order.Id);

                if (fullOrder == null)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Không thể load lại đơn hàng sau khi tạo."));

                var response = _mapper.Map<OrderRespondDTO>(fullOrder);
                return ApiResult<OrderRespondDTO>.Success(response, "Tạo order thành công!!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Đã có lỗi xảy ra khi tạo đơn hàng: " + ex.Message));
            }
        }



        //public async Task<ApiResult<OrderRespondDTO>> CreateOrderAsync(CreateOrderRequest request)
        //{
        //    try
        //    {
        //        var order = new Order
        //        {
        //            Id = Guid.NewGuid(),
        //            CustomerId = request.CustomerId,
        //            OrderDate = request.OrderDate,
        //            CreatedAt = _currentTime.GetVietnamTime(),
        //            CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty,
        //            OrderDetails = request.ServiceIds.Select(sid => new OrderDetail
        //            {
        //                ServiceId = sid,
        //                CreatedAt = _currentTime.GetVietnamTime(),
        //                CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty
        //            }).ToList()
        //        };

        //        await _repository.AddAsync(order);
        //        await _unitOfWork.SaveChangesAsync();

        //        var dto = _mapper.Map<OrderRespondDTO>(order);
        //        return ApiResult<OrderRespondDTO>.Success(dto, "Tạo đơn hàng thành công!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi tạo đơn hàng: " + ex.Message));
        //    }
        //}




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

        public async Task<ApiResult<OrderRespondDTO>> UpdateOrderById(UpdateOrderRequestDTO request)
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



        private async Task<Staff?> FindAvailableStaffAsync(Guid serviceId, DateTime start, DateTime end)
        {
            var staffs = await _staffCheduleRepository.GetAvailableStaffs(start, end);

            foreach (var staff in staffs)
            {
                var conflict = await _orderDetailRepo.HasConflict(staff.Id, start, end);
                if (!conflict)
                    return staff;
            }

            return null;
        }
    }
}
