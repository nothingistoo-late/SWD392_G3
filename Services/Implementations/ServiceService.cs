using AutoMapper;
using DTOs.ServiceDTO.Request;
using DTOs.ServiceDTO.Respond;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class ServiceService : BaseService<Service, Guid>, IServiceService
    {
        private readonly IMapper _mapper;
        public ServiceService(IGenericRepository<Service, Guid> repository, 
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork, 
            ICurrentTime currentTime,
            IMapper mapper) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
        }

        public async Task<ApiResult<ServiceRespondDTO>> CreateServiceAsync(AddServiceRequestDTO request)
        {
            try
            {
                var service = _mapper.Map<Service>(request);
                service.CreatedBy = _currentUserService.GetUserId() ?? Guid.Empty;
                service.CreatedAt = _currentTime.GetVietnamTime();
                await CreateAsync(service);
                return ApiResult<ServiceRespondDTO>.Success(_mapper.Map<ServiceRespondDTO>(service),"Tạo service thành công!!");
            }
            catch (Exception e){
                return ApiResult<ServiceRespondDTO>.Failure(new Exception("Lỗi khi tạo service!!"+  e.Message));
            }
            
        }

        public async Task<ApiResult<List<ServiceRespondDTO>>> GetAllServicesAsync()
        {
            try
            {
                var services = await _repository.GetAllAsync(s => !s.IsDeleted);
                var serviceDTOs = _mapper.Map<List<ServiceRespondDTO>>(services);
                return ApiResult<List<ServiceRespondDTO>>.Success(serviceDTOs, "Lấy danh sách dịch vụ thành công!!");
            }
            catch (Exception e)
            {
                return ApiResult<List<ServiceRespondDTO>>.Failure(new Exception("Lỗi khi lấy danh sách dịch vụ!!" + e.Message));
            }
        }

        public async Task<ApiResult<ServiceRespondDTO>> GetServiceByIdAsync(Guid Id)
        {
            try
            {
                var service = await _repository.GetByIdAsync(Id);
                if (service.IsDeleted)
                {
                    return ApiResult<ServiceRespondDTO>.Failure(new Exception("Không tìm thấy dịch vụ (có thể đã bị xóa)."));
                }

                if (service == null)
                {
                    return ApiResult<ServiceRespondDTO>.Failure(new Exception("Không tìm thấy dịch vụ với Id: " + Id));
                }
                var serviceDTO = _mapper.Map<ServiceRespondDTO>(service);
                return ApiResult<ServiceRespondDTO>.Success(serviceDTO, "Lấy dịch vụ thành công!!");
            }
            catch (Exception e)
            {
                return ApiResult<ServiceRespondDTO>.Failure(new Exception("Lỗi khi lấy dịch vụ!!" + e.Message));
            }
        }

        public async Task<ApiResult<ServiceRespondDTO>> SoftDeleteServiceById(Guid serviceId)
        {
            try
            {

                var service = await _repository.GetByIdAsync(serviceId);

                var deleted = await _repository.SoftDeleteAsync(serviceId, _currentUserService.GetUserId());
                if (!deleted)
                    return ApiResult<ServiceRespondDTO>.Failure(new Exception($"Không tìm thấy dịch vụ với Id: {serviceId}"));

                await _unitOfWork.SaveChangesAsync();

                var dto = _mapper.Map<ServiceRespondDTO>(service);
                return ApiResult<ServiceRespondDTO>.Success(dto, "Xóa mềm dịch vụ thành công!");
            }
            catch (Exception ex)
            {
                return ApiResult<ServiceRespondDTO>.Failure(new Exception("Lỗi khi xóa mềm dịch vụ: " + ex.Message));
            }
        }

        public async Task<ApiResult<ServiceRespondDTO>> UpdateServiceById(UpdateServiceRequest request)
        {
            try
            {
                var service = await _repository.GetByIdAsync(request.Id);
                if (service.IsDeleted)
                {
                    return ApiResult<ServiceRespondDTO>.Failure(new Exception("Dịch vụ đã bị xóa và không thể cập nhật."));
                }

                if (service == null)
                {
                    return ApiResult<ServiceRespondDTO>.Failure(new Exception($"Service with ID {request.Id} not found."));
                }

                // Cập nhật các trường nếu có
                if (request.Name is not null)
                    service.Name = request.Name;

                if (request.Price.HasValue)
                    service.Price = request.Price.Value;

                if (request.Description is not null)
                    service.Description = request.Description;

                if (request.Duration.HasValue)
                    service.Duration = request.Duration.Value;

                service.UpdatedAt = _currentTime.GetVietnamTime();
                service.UpdatedBy = _currentUserService.GetUserId() ?? Guid.Empty;

                await UpdateAsync(service);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = _mapper.Map<ServiceRespondDTO>(service);
                return ApiResult<ServiceRespondDTO>.Success(resultDto, "Update thành công!!");
            }
            catch (Exception e)
            {
                return ApiResult<ServiceRespondDTO>.Failure(new Exception("Lỗi khi cập nhật dịch vụ!!" + e.Message));
            }
        }


        public async Task<ApiResult<List<ServiceRespondDTO>>> GetServicesByFilterAsync(ServiceFilterDTO filter)
        {
            // 👉 Validate tay cho chắc
            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
                return ApiResult<List<ServiceRespondDTO>>.Failure(new Exception("MinPrice không được lớn hơn MaxPrice."));

            if (filter.MinDuration.HasValue && filter.MaxDuration.HasValue && filter.MinDuration > filter.MaxDuration)
                return ApiResult<List<ServiceRespondDTO>>.Failure(new Exception("MinDuration không được lớn hơn MaxDuration."));

            try
            {
                var services = await _unitOfWork.ServiceRepository.GetAllAsync(
                    predicate: s =>
                        (filter.IncludeDeleted || !s.IsDeleted) &&
                        (string.IsNullOrWhiteSpace(filter.Name) || s.Name.ToLower().Contains(filter.Name.ToLower())) &&
                        (!filter.MinPrice.HasValue || s.Price >= filter.MinPrice.Value) &&
                        (!filter.MaxPrice.HasValue || s.Price <= filter.MaxPrice.Value) &&
                        (!filter.MinDuration.HasValue || s.Duration >= filter.MinDuration.Value) &&
                        (!filter.MaxDuration.HasValue || s.Duration <= filter.MaxDuration.Value)
                );

                var mapped = _mapper.Map<List<ServiceRespondDTO>>(services);
                return ApiResult<List<ServiceRespondDTO>>.Success(mapped, "Lọc và lấy dịch vụ theo bộ lọc thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<ServiceRespondDTO>>.Failure(new Exception("Lỗi khi lọc dịch vụ: " + ex.Message));
            }
        }


    }
}
