using AutoMapper;
using DTOs;
using DTOs.Customer.Responds;
using DTOs.OrderDTO.Respond;
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
            CreateMap<Order, OrderRespondDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.OrderDetails.Select(od => od.Service)))
                .ReverseMap();
            CreateMap<Customer, CustomerRespondDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender.ToString())).ReverseMap();
        }
    }
}
