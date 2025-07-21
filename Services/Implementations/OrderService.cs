using AutoMapper;
using DTOs.OrderDTO.Request;
using DTOs.OrderDTO.Respond;
using Microsoft.EntityFrameworkCore;
using Repositories;
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
        private readonly SWD392_G3DBcontext _context;

        public OrderService(SWD392_G3DBcontext context, IOrderDetailRepository orderDetailRepository,  IServiceRepository serviceRepository, IMapper mapper, IOrderRepository orderRepository, IGenericRepository<Order, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _serviceRepository = serviceRepository;
            _context = context;
        }

        public async Task<ApiResult<OrderRespondDTO>> CreateOrderAsync(CreateOrderRequestDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // ✅ 1. Kiểm tra customer
                var customer = await _unitOfWork.UserRepository.GetUserDetailsByIdAsync(dto.CustomerId);
                if (customer == null || customer.IsDeleted)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception($"Khách hàng với ID {dto.CustomerId} không tồn tại."));


                // ✅ 2. Khởi tạo order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    OrderDate = dto.OrderDate,
                    Status = OrderStatus.Pending,
                    OrderDetails = new List<OrderDetail>()
                };

                double total = 0;
                var staffScheduleTracker = new Dictionary<Guid, List<(DateTime start, DateTime end)>>();

                // ✅ 3. Duyệt từng dịch vụ
                foreach (var item in dto.Services)
                {
                    var service = await _serviceRepository.GetByIdAsync(item.ServiceId);
                    if (service == null || service.IsDeleted)
                        return ApiResult<OrderRespondDTO>.Failure(new Exception($"Dịch vụ với ID {item.ServiceId} không tồn tại."));

                    if (item.ScheduledTime < _currentTime.GetVietnamTime())
                        return ApiResult<OrderRespondDTO>.Failure(new Exception("Ngày đặt dịch vụ có tên " + service.Name + " không được trong quá khứ."));

                    var start = item.ScheduledTime;
                    var end = start.AddMinutes(service.Duration);

                    // ✅ 4. Check thời gian có hợp lệ không (giờ làm việc: 08:00–18:00)
                    if (start.Hour < 8 || end.Hour >= 18 || (end.Hour == 17 && end.Minute > 59))
                    {
                        return ApiResult<OrderRespondDTO>.Failure(
                            new Exception($"Giờ đặt {start:HH:mm} – {end:HH:mm} không nằm trong khung giờ làm việc (08:00 – 18:00)."));
                    }

                    // ✅ 5. Tìm staff rảnh có ca làm
                    var allStaff = await _unitOfWork.StaffRepository.GetAllAsync();
                    Staff? selectedStaff = null;

                    foreach (var staff in allStaff.Where(s => !s.IsDeleted))
                    {
                        var isBusy = await _unitOfWork.OrderDetailRepository.IsStaffBusy(staff.Id, start, end);
                        var tempBusy = staffScheduleTracker.TryGetValue(staff.Id, out var slots)
                                       && slots.Any(slot => slot.start < end && slot.end > start);
                        var isWorking = await _unitOfWork.StaffScheduleRepository
                            .IsWithinWorkingHours(staff.Id, start, end); // ✅ Hàm check ca làm thực tế

                        if (!isBusy && !tempBusy && isWorking)
                        {
                            selectedStaff = staff;

                            if (!staffScheduleTracker.ContainsKey(staff.Id))
                                staffScheduleTracker[staff.Id] = new List<(DateTime, DateTime)>();
                            staffScheduleTracker[staff.Id].Add((start, end));
                            break;
                        }
                    }

                    if (selectedStaff == null)
                        return ApiResult<OrderRespondDTO>.Failure(
                            new Exception($"Không có nhân viên nào rảnh và đang làm việc lúc {start:HH:mm dd/MM}."));

                    // ✅ 6. Add OrderDetail
                    order.OrderDetails.Add(new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = order.Id,
                        ServiceId = service.Id,
                        StaffId = selectedStaff.Id,
                        ScheduleTime = start
                    });

                    total += service.Price;
                }

                // ✅ 7. Lưu & commit
                order.TotalPrice = total;
                await CreateAsync(order);
                await _unitOfWork.CommitTransactionAsync();

                // ✅ 8. Load lại đầy đủ để trả về
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
                if (order == null || order.IsDeleted)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Đơn hàng không tồn tại hoặc đã bị xóa!"));
                var deleted = await _repository.SoftDeleteAsync(orderId, _currentUserService.GetUserId());
                if (!deleted)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Không tìm thấy đơn hàng để xóa!"));

                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<OrderRespondDTO>(order);
                return ApiResult<OrderRespondDTO>.Success(dto, "Xóa mềm đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Lỗi khi xóa mềm đơn hàng: " + ex.Message));
            }
        }
        public async Task<ApiResult<OrderRespondDTO>> UpdateOrderAsync(UpdateOrderRequestDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetFullOrderByIdAsync(dto.OrderId);
                if (order == null || order.IsDeleted)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Đơn hàng không tồn tại!"));

                if (order.Status != OrderStatus.Pending)
                    return ApiResult<OrderRespondDTO>.Failure(new Exception("Chỉ có thể cập nhật đơn hàng đang chờ xử lý."));

                // ✅ Cập nhật customer nếu có
                if (dto.CustomerId.HasValue && dto.CustomerId != order.CustomerId)
                {
                    var customer = await _unitOfWork.UserRepository.GetUserDetailsByIdAsync(dto.CustomerId.Value);
                    if (customer == null || customer.IsDeleted)
                        return ApiResult<OrderRespondDTO>.Failure(new Exception("Khách hàng không tồn tại."));

                    order.CustomerId = dto.CustomerId.Value;
                }

                var staffScheduleTracker = new Dictionary<Guid, List<(DateTime start, DateTime end)>>();

                // ✅ Duyệt các OrderDetail cũ để tính lại tổng tiền và build tracker
                double total = 0;
                foreach (var od in order.OrderDetails)
                {
                    var service = await _serviceRepository.GetByIdAsync(od.ServiceId);
                    if (service != null && !service.IsDeleted)
                    {
                        total += service.Price;

                        if (!staffScheduleTracker.ContainsKey(od.StaffId))
                            staffScheduleTracker[od.StaffId] = new();
                        staffScheduleTracker[od.StaffId].Add((od.ScheduleTime, od.ScheduleTime.AddMinutes(service.Duration)));
                    }
                }

                // ✅ Patch dịch vụ nếu được gửi lên
                if (dto.Services != null && dto.Services.Any())
                {
                    var existingDetailKeys = order.OrderDetails
                        .Select(x => $"{x.ServiceId}_{x.ScheduleTime:O}")
                        .ToHashSet();

                    foreach (var item in dto.Services)
                    {
                        var key = $"{item.ServiceId}_{item.ScheduledTime:O}";
                        if (existingDetailKeys.Contains(key)) continue; // đã có rồi → skip

                        var service = await _serviceRepository.GetByIdAsync(item.ServiceId);
                        if (service == null || service.IsDeleted)
                            return ApiResult<OrderRespondDTO>.Failure(new Exception($"Dịch vụ {item.ServiceId} không tồn tại."));

                        var start = item.ScheduledTime;
                        var end = start.AddMinutes(service.Duration);

                        if (start.Hour < 8 || end.Hour >= 18)
                            return ApiResult<OrderRespondDTO>.Failure(new Exception($"Giờ {start:HH:mm} – {end:HH:mm} không hợp lệ."));

                        var allStaff = await _unitOfWork.StaffRepository.GetAllAsync();
                        Staff? selectedStaff = null;

                        foreach (var staff in allStaff.Where(s => !s.IsDeleted))
                        {
                            var isBusy = await _unitOfWork.OrderDetailRepository.IsStaffBusy(staff.Id, start, end);
                            var tempBusy = staffScheduleTracker.TryGetValue(staff.Id, out var slots)
                                           && slots.Any(slot => slot.start < end && slot.end > start);
                            var isWorking = await _unitOfWork.StaffScheduleRepository
                                .IsWithinWorkingHours(staff.Id, start, end);

                            if (!isBusy && !tempBusy && isWorking)
                            {
                                selectedStaff = staff;

                                if (!staffScheduleTracker.ContainsKey(staff.Id))
                                    staffScheduleTracker[staff.Id] = new();
                                staffScheduleTracker[staff.Id].Add((start, end));
                                break;
                            }
                        }

                        if (selectedStaff == null)
                            return ApiResult<OrderRespondDTO>.Failure(
                                new Exception($"Không có nhân viên nào rảnh lúc {start:HH:mm dd/MM}."));

                        var detail = new OrderDetail
                        {
                            OrderDetailId = Guid.NewGuid(),
                            OrderId = order.Id,
                            ServiceId = service.Id,
                            StaffId = selectedStaff.Id,
                            ScheduleTime = start
                        };

                        await _unitOfWork.OrderDetailRepository.AddAsync(detail);
                        order.OrderDetails.Add(detail);
                        total += service.Price;
                    }

                    // ✅ Update lại total price nếu có thêm dịch vụ
                    order.TotalPrice = total;
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.CommitTransactionAsync();

                var fullOrder = await _unitOfWork.OrderRepository.GetFullOrderByIdAsync(order.Id);
                var response = _mapper.Map<OrderRespondDTO>(fullOrder);

                return ApiResult<OrderRespondDTO>.Success(response, "Cập nhật đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Có lỗi khi cập nhật đơn hàng: " + ex.Message));
            }
        }

        public async Task<ApiResult<bool>> MarkWholeOrderCompletedAsync(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetFullOrderByIdAsync(orderId);
                if (order == null || order.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy order."));

                foreach (var detail in order.OrderDetails.Where(d => !d.IsDeleted))
                {
                    detail.Status = OrderDetailStatus.Completed;
                }

                order.Status = OrderStatus.Done;

                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, "Đã đánh dấu toàn bộ đơn hàng là hoàn thành.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi cập nhật đơn hàng: " + ex.Message));
            }
        }

        public async Task<ApiResult<bool>> CancelWholeOrderAsync(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetFullOrderByIdAsync(orderId);
                if (order == null || order.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Không tìm thấy order."));

                foreach (var detail in order.OrderDetails.Where(d => !d.IsDeleted))
                {
                    detail.Status = OrderDetailStatus.Cancelled;
                }

                order.Status = OrderStatus.Cancelled;

                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, "Đã huỷ toàn bộ đơn hàng.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi huỷ đơn hàng: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<OrderRespondDTO>>> GetOrdersByStaffIdAsync(Guid staffId)
        {
            var orders = await _context.Orders
                .Where(o => !o.IsDeleted && o.OrderDetails.Any(od => od.StaffId == staffId && !od.IsDeleted))
                .Include(o => o.Customer)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderDetails
                    .Where(od => !od.IsDeleted && od.StaffId == staffId)) // chỉ lấy các OrderDetail của staff này
                    .ThenInclude(od => od.Service)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Staff)
                        .ThenInclude(s => s.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            var result = _mapper.Map<List<OrderRespondDTO>>(orders);
            if (result == null || !result.Any())
            {
                return ApiResult<List<OrderRespondDTO>>.Failure(new Exception("Không tìm thấy đơn hàng cho nhân viên này."));
            }
            return ApiResult<List<OrderRespondDTO>>.Success(result, "Lấy đơn hàng của nhân viên thành công!");
        }

        public async Task<ApiResult<List<OrderRespondDTO>>> GetAllOrdersByCustomerIdAsync(Guid customerId)
        {
            try
            {
                var orders = await _orderRepository.GetAllWithCustomerIdAndServiceAsync(customerId);
                if (orders == null || !orders.Any())
                    return ApiResult<List<OrderRespondDTO>>.Failure(new Exception("Không tìm thấy đơn hàng cho khách hàng này!"));
                var result = _mapper.Map<List<OrderRespondDTO>>(orders);
                return ApiResult<List<OrderRespondDTO>>.Success(result, "Lấy tất cả đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrderRespondDTO>>.Failure(new Exception("Lỗi khi lấy danh sách đơn hàng: " + ex.Message));
            }
        }

    }
}
