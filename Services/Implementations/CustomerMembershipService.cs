using AutoMapper;
using DTOs.CustomerMembership.Respond;
using DTOs.OrderDTO.Respond;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class CustomerMembershipService : BaseService<CustomerMembership, Guid>, ICustomerMembershipService
    {

        private readonly IMapper _mapper;
        public CustomerMembershipService(IMapper mapper, IGenericRepository<CustomerMembership, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
        }


        public async Task<ApiResult<OrderRespondDTO>> CreateMembershipOrderAsync(Guid customerId, Guid membershipId)
        {
            var membership = await _unitOfWork.MembershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Membership not found"));

            var customer = await _unitOfWork.CustomerRepository.FirstOrDefaultAsync(x => x.UserId == customerId);
            if (customer == null)
                return ApiResult<OrderRespondDTO>.Failure(new Exception("Customer not found"));

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.UserId,
                TotalPrice = (double)membership.Price,
                Type = OrderType.Membership,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<OrderRespondDTO>(order);
            return ApiResult<OrderRespondDTO>.Success(result, "Order created successfully.");

        }


        public async Task<ApiResult<CustomerMembershipWithOrderResponse>> CreateMembershipOrderForCustomerAsync(Guid customerId, Guid membershipId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {

                var currentCustomer = _currentUserService.GetUserId();
                //if (currentCustomer != customerId)
                //    return ApiResult<CustomerMembershipWithOrderResponse>.Failure(new Exception("Bạn không có quyền thực hiện hành động này cho khách hàng khác."));

                if (await _unitOfWork.CustomerRepository.AnyAsync(o => o.UserId == customerId) == false)
                    return ApiResult<CustomerMembershipWithOrderResponse>.Failure(new Exception("Không tìm thấy khách hàng!! Hãy thử đăng nhập và thử lại!!"));

                var membership = await _unitOfWork.MembershipRepository.GetByIdAsync(membershipId);

                if (membership == null)
                    return ApiResult<CustomerMembershipWithOrderResponse>.Failure(new Exception("Không tìm thấy gói membership."));

                // 2. Gọi lại hàm AddMembershipToCustomerAsync
                var addResult = await AddMembershipToCustomerAsync(customerId, membershipId);
                if (!addResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    //return addResult; // trả về lỗi luôn
                    return ApiResult<CustomerMembershipWithOrderResponse>.Failure(new Exception(addResult.Message));

                }

                // 1. Tạo Order mới
                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending, // hoặc Paid nếu muốn tính là đã thanh toán luôn
                    TotalPrice = (double)membership.Price, // Có thể lấy từ Membership.Price nếu muốn tính luôn
                    Notes = "Đơn hàng mua gói Membership"
                };

                await _unitOfWork.OrderRepository.AddAsync(newOrder);
                await _unitOfWork.SaveChangesAsync();

                // 3. Tạo bản ghi OrderMembership để liên kết đơn hàng và membership
                var orderMembership = new OrderMembership
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    MembershipId = membershipId
                };

                await _unitOfWork.OrderMembershipRepository.AddAsync(orderMembership);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // Mapping kết quả
                var membershipDto = _mapper.Map<CustomerMembershipResponse>(addResult.Data);
                var orderDto = _mapper.Map<OrderResponse>(newOrder);

                // Gộp lại
                var fullResponse = new CustomerMembershipWithOrderResponse
                {
                    Membership = membershipDto,
                    Order = orderDto
                };

                return ApiResult<CustomerMembershipWithOrderResponse>.Success(fullResponse, "Tạo đơn hàng và gán membership thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<CustomerMembershipWithOrderResponse>.Failure(new Exception("Có lỗi xảy ra khi tạo đơn hàng membership: " + ex.Message));
            }
        }



        public async Task<ApiResult<CustomerMembershipResponse>> AddMembershipToCustomerAsync(Guid customerId, Guid membershipId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.AnyAsync(o => o.UserId == customerId);
                var membership = await _unitOfWork.MembershipRepository.GetByIdAsync(membershipId);

                if (customer == null || membership == null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Không tìm thấy khách hàng hoặc gói membership."));

                var existingActive = await _repository
                    .GetQueryable()
                    .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.EndDate == null);

                if (existingActive != null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Khách hàng đã có một membership đang hoạt động."));

                var newCM = new CustomerMembership
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    MembershipId = membershipId,
                    StartDate = DateTime.UtcNow,
                    EndDate = null
                };

                await _repository.AddAsync(newCM);
                await _unitOfWork.SaveChangesAsync();

                // Re-load để include Membership (tránh Membership == null khi map)
                var created = await _repository
                    .GetQueryable()
                    .Include(x => x.Membership)
                    .FirstAsync(x => x.Id == newCM.Id);

                var response = _mapper.Map<CustomerMembershipResponse>(created);
                return ApiResult<CustomerMembershipResponse>.Success(response, "Thêm membership thành công.");
            }
            catch (Exception)
            {
                return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Có lỗi xảy ra khi thêm membership cho khách hàng."));
            }
        }


        public async Task<ApiResult<CustomerMembershipResponse>> UpdateCustomerMembershipAsync(Guid cmId, Guid newMembershipId)
        {
            try
            {
                var cm = await _unitOfWork.CustomerMemberShipRepository.GetByIdAsync(cmId);
                var newMembership = await _unitOfWork.MembershipRepository.GetByIdAsync(newMembershipId);

                if (cm == null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Không tìm thấy bản ghi customer-membership."));

                if (newMembership == null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Gói membership mới không tồn tại."));

                cm.MembershipId = newMembershipId;
                cm.StartDate = DateTime.UtcNow;
                cm.EndDate = null;

                await _unitOfWork.CustomerMemberShipRepository.UpdateAsync(cm);
                await _unitOfWork.SaveChangesAsync();

                var updated = await _unitOfWork.CustomerMemberShipRepository
                    .GetQueryable()
                    .Include(x => x.Membership)
                    .FirstAsync(x => x.Id == cm.Id);

                var response = _mapper.Map<CustomerMembershipResponse>(updated);
                return ApiResult<CustomerMembershipResponse>.Success(response, "Cập nhật membership thành công.");
            }
            catch (Exception)
            {
                return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Lỗi khi cập nhật membership cho khách hàng."));
            }
        }

        public async Task<ApiResult<CustomerMembershipResponse>> EndCustomerMembershipAsync(Guid cmId)
        {
            try
            {
                var cm = await _unitOfWork.CustomerMemberShipRepository.GetByIdAsync(cmId);

                if (cm == null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Không tìm thấy membership của khách hàng."));

                if (cm.EndDate != null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Membership này đã kết thúc từ trước."));

                cm.EndDate = DateTime.UtcNow;

                await _unitOfWork.CustomerMemberShipRepository.UpdateAsync(cm);
                await _unitOfWork.SaveChangesAsync();

                var ended = await _unitOfWork.CustomerMemberShipRepository
                    .GetQueryable()
                    .Include(x => x.Membership)
                    .FirstAsync(x => x.Id == cm.Id);

                var response = _mapper.Map<CustomerMembershipResponse>(ended);
                return ApiResult<CustomerMembershipResponse>.Success(response, "Đã kết thúc membership.");
            }
            catch (Exception)
            {
                return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Có lỗi khi kết thúc membership."));
            }
        }

        public async Task<ApiResult<List<CustomerMembershipResponse>>> GetMembershipsByCustomerAsync(Guid customerId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.AnyAsync( o=>o.UserId == customerId);
                if (customer == null)
                    return ApiResult<List<CustomerMembershipResponse>>.Failure(new Exception("Không tìm thấy khách hàng."));

                var memberships = await _unitOfWork.CustomerMemberShipRepository
                    .GetQueryable()
                    .Where(x => x.CustomerId == customerId)
                    .Include(x => x.Membership)
                    .ToListAsync();

                if (memberships == null || !memberships.Any())
                    return ApiResult<List<CustomerMembershipResponse>>.Failure(new Exception("Khách hàng này không có membership nào."));

                var result = _mapper.Map<List<CustomerMembershipResponse>>(memberships);
                return ApiResult<List<CustomerMembershipResponse>>.Success(result,"Lấy tất cả membership theo khách hàng thành công!!");
            }
            catch (Exception)
            {
                return ApiResult<List<CustomerMembershipResponse>>.Failure(new Exception("Lỗi khi lấy danh sách membership của khách hàng."));
            }
        }
        public async Task<ApiResult<CustomerMembershipResponse>> GetBestActiveMembershipByCustomerAsync(Guid customerId)
        {
            try
            {
                var customerExists = await _unitOfWork.CustomerRepository
                    .AnyAsync(o => o.UserId == customerId);

                if (!customerExists)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Không tìm thấy khách hàng."));

                var bestMembership = await _unitOfWork.CustomerMemberShipRepository
                    .GetQueryable()
                    .Where(x => x.CustomerId == customerId &&(
                                x.EndDate > DateTime.Now || x.EndDate==null)&&
                                x.Membership.IsDeleted == false)
                    .Include(x => x.Membership)
                    .OrderByDescending(x => x.Membership.Price)  // Nếu trùng Level thì lấy cái đắt hơn
                    .FirstOrDefaultAsync();

                if (bestMembership == null)
                    return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Không có membership nào đang hoạt động."));

                var result = _mapper.Map<CustomerMembershipResponse>(bestMembership);
                return ApiResult<CustomerMembershipResponse>.Success(result, "Lấy membership VIP nhất còn hoạt động thành công!");
            }
            catch (Exception)
            {
                return ApiResult<CustomerMembershipResponse>.Failure(new Exception("Lỗi khi lấy membership VIP."));
            }
        }

    }
}
