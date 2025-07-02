using AutoMapper;
using DTOs.StaffDTO.Request;
using DTOs.StaffDTO.Respond;
using Microsoft.AspNetCore.Identity;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class StaffService : BaseService<Staff, Guid>, IStaffService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public StaffService(UserManager<User> userManager,IMapper mapper, IGenericRepository<Staff, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
            _userManager = userManager;

        }

        // ✅ 1. Tạo nhân viên (User + Staff)
        public async Task<ApiResult<StaffRespondDTO>> CreateStaffAsync(CreateStaffRequestDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return ApiResult<StaffRespondDTO>.Failure(new Exception("Email đã tồn tại trong hệ thống!! Vui lòng sử dụng mail khác!!"));

                // Tạo User
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    UserName = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResult<StaffRespondDTO>.Failure(new Exception("Tạo user thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description))));
                }

                // Tạo Staff
                var staff = new Staff
                {
                    Id = user.Id,
                    HireDate = dto.HireDate,
                    Salary = dto.Salary,
                    ImgURL = dto.ImgURL,
                    Note = dto.Note
                };

                //await _unitOfWork.StaffRepository.AddAsync(staff);
                await CreateAsync(staff);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var staffDTO = _mapper.Map<StaffRespondDTO>(staff);
                return ApiResult<StaffRespondDTO>.Success(staffDTO, "Tạo nhân viên thành công!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResult<StaffRespondDTO>.Failure(new Exception("Có lỗi xảy ra khi tạo nhân viên: " + ex.Message));
            }
        }

        // ✅ 2. Cập nhật nhân viên (partial update)
        public async Task<ApiResult<StaffRespondDTO>> UpdateStaffAsync(Guid staffId, UpdateStaffRequestDTO dto)
        {
            try
            {
                var staff = await _unitOfWork.StaffRepository.GetByIdAsync(staffId, s => s.User);
                if (staff == null || staff.IsDeleted)
                    return ApiResult<StaffRespondDTO>.Failure(new Exception("Nhân viên không tồn tại hoặc đã bị xoá."));

                // Update User info
                if (!string.IsNullOrWhiteSpace(dto.FirstName)) staff.User.FirstName = dto.FirstName;
                if (!string.IsNullOrWhiteSpace(dto.LastName)) staff.User.LastName = dto.LastName;
                if (!string.IsNullOrWhiteSpace(dto.Gender)) staff.User.Gender = dto.Gender;

                // Update Staff info
                if (dto.Salary.HasValue) staff.Salary = dto.Salary.Value;
                if (dto.HireDate.HasValue) staff.HireDate = dto.HireDate.Value;
                if (!string.IsNullOrWhiteSpace(dto.ImgURL)) staff.ImgURL = dto.ImgURL;
                if (!string.IsNullOrWhiteSpace(dto.Note)) staff.Note = dto.Note;

                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<StaffRespondDTO>(staff);
                return ApiResult<StaffRespondDTO>.Success(result, "Cập nhật thông tin nhân viên thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<StaffRespondDTO>.Failure(new Exception("Có lỗi xảy ra khi cập nhật nhân viên: " + ex.Message));
            }
        }

        // ✅ 3. Soft delete
        public async Task<ApiResult<StaffRespondDTO>> SoftDeleteStaffAsync(Guid staffId)
        {
            try
            {
                var staff = await _unitOfWork.StaffRepository.GetByIdAsync(staffId, includes: s=>s.User);
                if (staff == null || staff.IsDeleted)
                    return ApiResult<StaffRespondDTO>.Failure(new Exception("Nhân viên không tồn tại hoặc đã bị xoá."));

                staff.IsDeleted = true;
                staff.User.IsDeleted = true; // Xoá mềm User liên quan
                await _unitOfWork.SaveChangesAsync();
                var respond = _mapper.Map<StaffRespondDTO>(staff);
                return ApiResult<StaffRespondDTO>.Success(respond,"Xoá mềm nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<StaffRespondDTO>.Failure(new Exception("Lỗi khi xoá nhân viên: " + ex.Message));
            }
        }

        // ✅ 4. Get all active staff (include User)
        public async Task<ApiResult<List<StaffRespondDTO>>> GetAllStaffAsync()
        {
            try
            {
                var staffs = await _unitOfWork.StaffRepository
                    .GetAllAsync(s => !s.IsDeleted, includes: s => s.User);

                if (staffs == null || !staffs.Any())
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("Không có nhân viên nào trong hệ thống."));

                var result = _mapper.Map<List<StaffRespondDTO>>(staffs);
                return ApiResult<List<StaffRespondDTO>>.Success(result,"Lấy danh sách nhân viên thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("Lỗi khi lấy danh sách nhân viên: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<StaffRespondDTO>>> GetStaffByFilterAsync(StaffFilterDTO filter)
        {
            try
            {
                // 🚫 Manual validation
                if (filter.MinSalary.HasValue && filter.MinSalary < 0)
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("MinSalary không được nhỏ hơn 0."));

                if (filter.MaxSalary.HasValue && filter.MaxSalary < 0)
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("MaxSalary không được nhỏ hơn 0."));

                if (filter.MinSalary.HasValue && filter.MaxSalary.HasValue &&
                    filter.MinSalary > filter.MaxSalary)
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("MinSalary không được lớn hơn MaxSalary."));

                if (filter.HireDateFrom.HasValue && filter.HireDateTo.HasValue &&
                    filter.HireDateFrom > filter.HireDateTo)
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("HireDateFrom không được sau HireDateTo."));

                var staffs = await _unitOfWork.StaffRepository.GetAllAsync(
                    predicate: s =>
                        (string.IsNullOrEmpty(filter.Name) || s.User.FullName.ToLower().Contains(filter.Name.ToLower())) &&
                        (!filter.MinSalary.HasValue || s.Salary >= filter.MinSalary.Value) &&
                        (!filter.MaxSalary.HasValue || s.Salary <= filter.MaxSalary.Value) &&
                        (!filter.HireDateFrom.HasValue || s.HireDate >= filter.HireDateFrom.Value) &&
                        (!filter.HireDateTo.HasValue || s.HireDate <= filter.HireDateTo.Value),
                    includes: s => s.User
                );

                if (staffs == null || !staffs.Any())
                    return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("Không tìm thấy nhân viên nào phù hợp với bộ lọc."));

                var respond = _mapper.Map<List<StaffRespondDTO>>(staffs);
                return ApiResult<List<StaffRespondDTO>>.Success(respond,"Lọc và lấy nhân viên theo bộ lọc thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<StaffRespondDTO>>.Failure(new Exception("Lỗi khi lọc nhân viên: " + ex.Message));
            }
        }

        // ✅ 5. Soft Delete 

        public async Task<ApiResult<BulkStaffDeleteResultDTO>> SoftDeleteManyStaffAsync(List<Guid> staffIds)
        {
            var resultDto = new BulkStaffDeleteResultDTO();

            foreach (var staffId in staffIds)
            {
                try
                {
                    var staff = await _unitOfWork.StaffRepository.GetByIdAsync(staffId, includes: s => s.User);

                    if (staff == null)
                    {
                        resultDto.Failed.Add(new FailedStaffDeleteDTO
                        {
                            StaffId = staffId,
                            Reason = "Không tìm thấy nhân viên."
                        });
                        continue;
                    }

                    if (staff.IsDeleted)
                    {
                        resultDto.Failed.Add(new FailedStaffDeleteDTO
                        {
                            StaffId = staffId,
                            Reason = "Nhân viên đã bị xoá trước đó."
                        });
                        continue;
                    }

                    staff.IsDeleted = true;
                    resultDto.Deleted.Add(_mapper.Map<StaffRespondDTO>(staff));
                }
                catch (Exception ex)
                {
                    resultDto.Failed.Add(new FailedStaffDeleteDTO
                    {
                        StaffId = staffId,
                        Reason = "Lỗi: " + ex.Message
                    });
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ApiResult<BulkStaffDeleteResultDTO>.Failure(new Exception("Lỗi khi lưu thay đổi: " + ex.Message));
            }

            return ApiResult<BulkStaffDeleteResultDTO>.Success(resultDto, "Xoá mềm nhiều nhân viên đã xử lý xong.");
        }

    }
}
