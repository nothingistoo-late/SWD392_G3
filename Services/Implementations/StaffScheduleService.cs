using AutoMapper;
using DTOs.StaffDTO.Respond;
using DTOs.StaffScheduleDTO.Request;
using DTOs.StaffScheduleDTO.Respond;
using Repositories.Interfaces;
using Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class StaffScheduleService : BaseService<StaffSchedule, Guid>, IStaffScheduleService
    {
        private readonly IMapper _mapper;

        public StaffScheduleService(IMapper mapper,IGenericRepository<StaffSchedule, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;

        }

        public async Task<ApiResult<StaffScheduleRespondDTO>> CreateScheduleAsync(CreateStaffScheduleRequestDTO dto)
        {
            try
            {
                var staff = await _unitOfWork.StaffRepository.GetByIdAsync(dto.StaffId);
                if (staff == null) return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Nhân viên không tồn tại."));

                var schedule = new StaffSchedule
                {
                    StaffScheduleId = Guid.NewGuid(),
                    StaffId = dto.StaffId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };

                await _unitOfWork.StaffScheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                var respond = _mapper.Map<StaffScheduleRespondDTO>(schedule);

                return ApiResult<StaffScheduleRespondDTO>.Success(respond, "Tạo lịch làm việc thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Có lỗi xảy ra khi tạo lịch làm việc: " + ex.Message));
            }
        }
        public async Task<ApiResult<List<StaffScheduleRespondDTO>>> GetSchedulesByStaffIdAsync(Guid staffId)
        {

            var schedules = await _unitOfWork.StaffScheduleRepository
                .GetAllAsync(s => s.StaffId == staffId);

            var result = _mapper.Map<List<StaffScheduleRespondDTO>>(schedules);
            return ApiResult<List<StaffScheduleRespondDTO>>.Success(result,"Lấy lịch của staff thành công!!");
        }
    }
}
