using AutoMapper;
using DTOs.MemberShip.Request;
using DTOs.MemberShip.Respond;
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
    public class MembershipService : BaseService<Membership, Guid>, IMembershipService
    {

        private readonly IMapper _mapper;

        public MembershipService(IMapper mapper,IGenericRepository<Membership, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
        }

        public async Task<ApiResult<List<MembershipResponse>>> CreateRangeAsync(List<CreateMembershipRequest> dtos)
        {
            if (dtos == null || !dtos.Any())
                return ApiResult<List<MembershipResponse>>.Failure(new Exception("Danh sách gói hội viên rỗng."));

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ⚠️ Validate từng thằng một
                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.Name) || dto.DiscountPercentage <= 0 || dto.DurationInDays <= 0)
                    {
                        return ApiResult<List<MembershipResponse>>.Failure(new Exception($"Gói '{dto.Name}' không hợp lệ."));
                    }

                    var exist = await _repository.AnyAsync(m => m.Name == dto.Name);
                    if (exist)
                    {
                        return ApiResult<List<MembershipResponse>>.Failure(new Exception($"Gói '{dto.Name}' đã tồn tại."));
                    }
                }

                var entities = _mapper.Map<List<Membership>>(dtos);

                await _repository.AddRangeAsync(entities);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = _mapper.Map<List<MembershipResponse>>(entities);
                return ApiResult<List<MembershipResponse>>.Success(result,"Thêm loại membership thành công!!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<List<MembershipResponse>>.Failure(new Exception("Đã xảy ra lỗi khi tạo gói hội viên: " + ex.Message));
            }
        }



        public async Task<ApiResult<List<MembershipDeleteResultDTO>>> DeleteAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return ApiResult<List<MembershipDeleteResultDTO>>.Failure(new Exception("Danh sách ID rỗng."));

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            var resultList = new List<MembershipDeleteResultDTO>();

            try
            {
                foreach (var id in ids)
                {
                    var entity = await _repository.GetByIdAsync(id);

                    if (entity == null)
                    {
                        resultList.Add(new MembershipDeleteResultDTO
                        {
                            Id = id,
                            IsDeleted = false,
                            Message = "Không tìm thấy membership."
                        });
                        continue;
                    }

                    if (entity.IsDeleted)
                    {
                        resultList.Add(new MembershipDeleteResultDTO
                        {
                            Id = id,
                            IsDeleted = false,
                            Message = "Membership đã bị xóa trước đó."
                        });
                        continue;
                    }

                    entity.IsDeleted = true;
                    await _repository.UpdateAsync(entity);

                    resultList.Add(new MembershipDeleteResultDTO
                    {
                        Id = id,
                        IsDeleted = true,
                        Message = "Xóa thành công."
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResult<List<MembershipDeleteResultDTO>>.Success(resultList,"Xóa membership thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<List<MembershipDeleteResultDTO>>.Failure(new Exception("Lỗi khi xóa Memberships: " + ex.Message));
            }
        }


        public async Task<ApiResult<List<MembershipResponse>>> GetAllAsync()
        {
            try
            {
                var memberships = await _repository.GetAllAsync();
                if (memberships == null)
                    return ApiResult<List<MembershipResponse>>.Failure(new Exception("Không tìm thấy membership nào!!"));
                var result = _mapper.Map<List<MembershipResponse>>(memberships);
                return ApiResult<List<MembershipResponse>>.Success(result,"Lấy danh sách membership thành công!!");

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return ApiResult<List<MembershipResponse>>.Failure(new Exception("An error occurred while fetching memberships."));
            }
        }

        public async Task<ApiResult<MembershipResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var memberships = await _repository.GetByIdAsync(id);
                if (memberships == null)
                    return ApiResult<MembershipResponse>.Failure(new Exception("Không tìm thấy membership nào!!"));
                var result = _mapper.Map<MembershipResponse>(memberships);
                return ApiResult<MembershipResponse>.Success(result, "Lấy membership theo id thành công!!");

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return ApiResult<MembershipResponse>.Failure(new Exception("An error occurred while fetching memberships."));
            }
        }


        public async Task<ApiResult<MembershipResponse>> UpdateAsync(Guid id, CreateMembershipRequest dto)
        {
            if (dto == null)
                return ApiResult<MembershipResponse>.Failure(new Exception("Dữ liệu cập nhật không hợp lệ."));

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                {
                    return ApiResult<MembershipResponse>.Failure(new Exception("Không tìm thấy Membership để cập nhật."));
                }

                // Cập nhật thông tin
                _mapper.Map(dto, entity); // Map trực tiếp từ dto sang entity
                await _repository.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = _mapper.Map<MembershipResponse>(entity);
                return ApiResult<MembershipResponse>.Success(response, "Cập nhật Membership thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<MembershipResponse>.Failure(new Exception("Lỗi khi cập nhật Membership: " + ex.Message));
            }
        }

    }
}
