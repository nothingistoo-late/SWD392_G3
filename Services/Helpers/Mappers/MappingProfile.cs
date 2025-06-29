using AutoMapper;
using DTOs.ServiceDTO.Request;
using DTOs.ServiceDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service , AddServiceRequestDTO>().ReverseMap();
            CreateMap<Service, ServiceRespondDTO>().ReverseMap();

            // Thêm nhiều mapping khác nếu cần
        }
    }
}
