using DTOs.ServiceDTO.Request;
using DTOs.ServiceDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResult<ServiceRespondDTO>> CreateServiceAsync(AddServiceRequestDTO request);
        Task<ApiResult<ServiceRespondDTO>> GetServiceByIdAsync(Guid Id);
        Task<ApiResult<List<ServiceRespondDTO>>> GetAllServicesAsync();
        Task<ApiResult<ServiceRespondDTO>> UpdateServiceById(UpdateServiceRequest request);
        Task<ApiResult<ServiceRespondDTO>> SoftDeleteServiceById(Guid serviceId);
        Task<ApiResult<List<ServiceRespondDTO>>> GetServicesByFilterAsync(ServiceFilterDTO filter);

        Task<ApiResult<List<ServiceRatingResponseDTO>>> GetRatingsByServiceIdAsync(Guid serviceId);

    }
}
