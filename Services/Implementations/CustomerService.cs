using AutoMapper;
using DTOs;
using DTOs.Customer.Responds;
using Microsoft.AspNetCore.Identity;
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
    public class CustomerService : BaseService<Customer, Guid>, ICustomerService
    {


        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public CustomerService(IMapper mapper, UserManager<User> usermanager,IGenericRepository<Customer, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _userManager = usermanager;
            _mapper = mapper;   
        }

        public async Task<ApiResult<CreateCustomerRequestDTO>> CreateCustomerAsync(CreateCustomerRequestDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(); // 🚀 Mở transaction

                // Bước 1: Tạo User
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender.ToString()
                };

                var createUserResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createUserResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResult<CreateCustomerRequestDTO>.Failure(new Exception("Tạo user thất bại: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description))));
                }

                // Bước 2: Tạo Customer
                var customer = new Customer
                {
                    UserId = user.Id,
                    Address = dto.Address

                };

                await _repository.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                // ✅ Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                return ApiResult<CreateCustomerRequestDTO>.Success(dto,"Tạo User + Customer thành công!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResult<CreateCustomerRequestDTO>.Failure(new Exception("Có lỗi xảy ra: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<CustomerRespondDTO>>> GetAllCustomersAsync()
        {
            var customers = await _repository.GetAllAsync(
                            predicate: c => !c.IsDeleted,
                            includes: c => c.User);
            var result = _mapper.Map<List<CustomerRespondDTO>>(customers);
            return ApiResult<List<CustomerRespondDTO>>.Success(result,"Lấy thông tin thành công!!");
        }


        public async Task<ApiResult<CustomerRespondDTO>> GetCustomerByIdAsync(Guid Id)
        {
            try
            {
                var customer = await _repository.GetByIdAsync(Id,
                    c => c.User
                );

                if (customer == null || customer.IsDeleted)
                {
                    return ApiResult<CustomerRespondDTO>.Failure(new Exception("Không tìm thấy khách hàng!"));
                }

                var resultDto = _mapper.Map<CustomerRespondDTO>(customer);
                return ApiResult<CustomerRespondDTO>.Success(resultDto,"Tìm thấy khách hàng thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<CustomerRespondDTO>.Failure(new Exception("Lỗi khi lấy khách hàng! " + ex.Message));
            }
        }

        public async Task<ApiResult<CustomerRespondDTO>> SoftDeleteCustomerById(Guid customerId)
        {
            try
            {
                var customer = await _repository.GetByIdAsync(customerId, c => c.User);

                if (customer == null || customer.IsDeleted)
                {
                    return ApiResult<CustomerRespondDTO>.Failure(new Exception("Không tìm thấy khách hàng!"));
                }

                customer.IsDeleted = true;
                customer.DeletedAt = _currentTime.GetVietnamTime();
                customer.DeletedBy = _currentUserService.GetUserId();

                await _repository.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = _mapper.Map<CustomerRespondDTO>(customer);
                return ApiResult<CustomerRespondDTO>.Success(resultDto, "Xóa mềm khách hàng thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<CustomerRespondDTO>.Failure(new Exception("Lỗi khi xóa mềm khách hàng! " + ex.Message));
            }
        }
    }
}
