using AutoMapper;
using BusinessObjects;
using DTOs.StaffDTO.Respond;
using DTOs.StaffScheduleDTO.Request;
using DTOs.StaffScheduleDTO.Respond;
using Microsoft.EntityFrameworkCore;
using Repositories;
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
        private readonly SWD392_G3DBcontext _context;

        public StaffScheduleService(SWD392_G3DBcontext context, IMapper mapper,IGenericRepository<StaffSchedule, Guid> repository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICurrentTime currentTime) : base(repository, currentUserService, unitOfWork, currentTime)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApiResult<StaffScheduleRespondDTO>> CreateScheduleAsync(CreateStaffScheduleRequestDTO dto)
        {
            try
            {
                // ✅ Check nhân viên tồn tại
                var staff = await _unitOfWork.StaffRepository.GetByIdAsync(dto.StaffId);
                if (staff == null)
                    return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Nhân viên không tồn tại."));

                // ✅ Check thời gian hợp lệ
                if (dto.StartTime >= dto.EndTime)
                    return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc."));

                // ✅ Check trùng lịch trong cùng thứ
                var isOverlapping = await _unitOfWork.StaffScheduleRepository.AnyAsync(s =>
                    s.StaffId == dto.StaffId &&
                    s.DayOfWeek == dto.DayOfWeek &&
                    !s.IsDeleted &&
                    (
                        (dto.StartTime >= s.StartTime && dto.StartTime < s.EndTime) ||
                        (dto.EndTime > s.StartTime && dto.EndTime <= s.EndTime) ||
                        (dto.StartTime <= s.StartTime && dto.EndTime >= s.EndTime)
                    )
                );

                if (isOverlapping)
                    return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Lịch làm việc bị trùng với lịch đã tồn tại."));

                // ✅ Tạo mới schedule
                var schedule = new StaffSchedule
                {
                    StaffScheduleId = Guid.NewGuid(),
                    StaffId = dto.StaffId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Note = dto.Note // nếu có
                };

                await _unitOfWork.StaffScheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var respond = _mapper.Map<StaffScheduleRespondDTO>(schedule);
                return ApiResult<StaffScheduleRespondDTO>.Success(respond, "Tạo lịch làm việc thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception($"Lỗi khi tạo lịch làm việc: {ex.Message}"));
            }
        }

        public async Task<ApiResult<StaffScheduleRespondDTO>> UpdateScheduleAsync(Guid scheduleId, UpdateStaffScheduleRequestDTO dto)
        {
            try
            {
                var schedule = await _unitOfWork.StaffScheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null || schedule.IsDeleted)
                    return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Lịch làm việc không tồn tại hoặc đã bị xoá."));

                if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
                    return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc."));

                if (dto.DayOfWeek.HasValue)
                    schedule.DayOfWeek = dto.DayOfWeek.Value;

                if (dto.StartTime.HasValue)
                    schedule.StartTime = dto.StartTime.Value;

                if (dto.EndTime.HasValue)
                    schedule.EndTime = dto.EndTime.Value;

                if (!string.IsNullOrWhiteSpace(dto.Note))
                    schedule.Note = dto.Note;

                await _unitOfWork.SaveChangesAsync();
                var respond = _mapper.Map<StaffScheduleRespondDTO>(schedule);
                return ApiResult<StaffScheduleRespondDTO>.Success(respond, "Cập nhật lịch làm việc thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<StaffScheduleRespondDTO>.Failure(new Exception("Lỗi khi cập nhật lịch làm việc: " + ex.Message));
            }
        }


        public async Task<ApiResult<List<StaffScheduleRespondDTO>>> GetSchedulesByStaffIdAsync(Guid staffId)
        {
            try
            {
                // Check staff tồn tại
                var staffExists = await _unitOfWork.StaffRepository.AnyAsync(s => s.Id == staffId && !s.IsDeleted);
                if (!staffExists)
                    return ApiResult<List<StaffScheduleRespondDTO>>.Failure(new Exception("Nhân viên không tồn tại hoặc đã bị xoá."));

                // Lấy lịch làm việc (không lấy lịch đã bị xoá)
                var schedules = await _unitOfWork.StaffScheduleRepository
                    .GetAllAsync(s => s.StaffId == staffId && !s.IsDeleted, includes: s => s.Staff.User);

                var result = _mapper.Map<List<StaffScheduleRespondDTO>>(schedules);
                return ApiResult<List<StaffScheduleRespondDTO>>.Success(result, "Lấy lịch của nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<StaffScheduleRespondDTO>>.Failure(new Exception("Lỗi khi lấy lịch làm việc: " + ex.Message));
            }
        }


        public async Task<ApiResult<bool>> SoftDeleteScheduleAsync(Guid scheduleId)
        {
            try
            {
                var schedule = await _unitOfWork.StaffScheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null || schedule.IsDeleted)
                    return ApiResult<bool>.Failure(new Exception("Lịch làm việc không tồn tại hoặc đã bị xoá."));

                schedule.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return ApiResult<bool>.Success(true, "Xoá mềm lịch làm việc thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failure(new Exception("Lỗi khi xoá mềm lịch làm việc: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<StaffScheduleRespondDTO>>> FilterSchedulesAsync(StaffScheduleFilterDTO filter)
        {
            try
            {
                var schedules = await _unitOfWork.StaffScheduleRepository.GetAllAsync(
                    predicate: s =>
                        (!filter.StaffId.HasValue || s.StaffId == filter.StaffId.Value) &&
                        (!filter.DayOfWeek.HasValue || s.DayOfWeek == filter.DayOfWeek.Value) &&
                        (!filter.TimeFrom.HasValue || s.StartTime >= filter.TimeFrom.Value) &&
                        (!filter.TimeTo.HasValue || s.EndTime <= filter.TimeTo.Value),
                    includes: s => s.Staff.User
                );

                if (schedules == null || !schedules.Any())
                    return ApiResult<List<StaffScheduleRespondDTO>>.Failure(new Exception("Không tìm thấy lịch làm việc phù hợp."));

                var result = _mapper.Map<List<StaffScheduleRespondDTO>>(schedules);
                return ApiResult<List<StaffScheduleRespondDTO>>.Success(result,"Lọc lịch làm việc của nhân viên thành công!!");
            }
            catch (Exception ex)
            {
                return ApiResult<List<StaffScheduleRespondDTO>>.Failure(new Exception("Lỗi khi lọc lịch làm việc: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<AvailableSlotDTO>>> GetAvailableSlotsAsync(StaffAvailableSlotsRequestDTO request)
        {
            try
            {
                var dayOfWeek = request.Date.DayOfWeek;

                // 1. Lấy ca làm
                var schedules = await _context.StaffSchedules
                    .Where(s => s.StaffId == request.StaffId &&
                                s.DayOfWeek == dayOfWeek &&
                                !s.IsDeleted)
                    .ToListAsync();

                // 2. Lấy các dịch vụ đã được đặt
                var startOfDay = request.Date.Date;
                var endOfDay = startOfDay.AddDays(1);

                var busySlots = await _context.OrderDetails
                        .Where(od => od.StaffId == request.StaffId &&
                                     od.ScheduleTime >= startOfDay &&
                                     od.ScheduleTime < endOfDay &&
                                     !od.IsDeleted &&
                                     od.Status != OrderDetailStatus.Completed &&
                                     od.Status != OrderDetailStatus.Cancelled)
                        .Select(od => new
                        {
                            Start = od.ScheduleTime.TimeOfDay,
                            End = od.ScheduleTime.TimeOfDay + TimeSpan.FromMinutes(od.Service.Duration)
                        })
                        .ToListAsync();

                // 3. Tính khoảng rảnh
                var availableSlots = new List<AvailableSlotDTO>();

                foreach (var schedule in schedules)
                {
                    var freeRanges = new List<(TimeSpan, TimeSpan)> { (schedule.StartTime, schedule.EndTime) };

                    foreach (var busy in busySlots)
                    {
                        freeRanges = freeRanges
                            .SelectMany(r => SubtractRange(r, (busy.Start, busy.End)))
                            .ToList();
                    }

                    availableSlots.AddRange(
                        freeRanges.Select(r => new AvailableSlotDTO
                        {
                            Start = TimeSpanToString(r.Item1),
                            End = TimeSpanToString(r.Item2)
                        })
                    );
                }

                return ApiResult<List<AvailableSlotDTO>>.Success(availableSlots, "Lấy danh sách thời gian rảnh thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<AvailableSlotDTO>>.Failure(new Exception("Lỗi khi lấy thời gian rảnh: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<AvailableStaffDTO>>> GetAvailableStaffInTimeRangeAsync(AvailableStaffRequestDTO dto)
        {
            try
            {
                var dayOfWeek = dto.Start.DayOfWeek;
                var startTime = dto.Start.TimeOfDay;
                var endTime = dto.End.TimeOfDay;

                var allStaffs = await _context.Staffs
                    .Include(s => s.User)
                    .Where(s => !s.IsDeleted)
                    .ToListAsync();

                var result = new List<AvailableStaffDTO>();

                foreach (var staff in allStaffs)
                {
                    // 1. Có ca làm phù hợp không?
                    var hasValidShift = await _context.StaffSchedules.AnyAsync(s =>
                        s.StaffId == staff.Id &&
                        s.DayOfWeek == dayOfWeek &&
                        !s.IsDeleted &&
                        s.StartTime <= startTime &&
                        s.EndTime >= endTime);

                    if (!hasValidShift)
                        continue;

                    // 2. Nhân viên có bận không?
                    var isBusy = await _context.OrderDetails.AnyAsync(od =>
                        od.StaffId == staff.Id &&
                        !od.IsDeleted &&
                        od.ScheduleTime < dto.End &&
                        od.ScheduleTime.AddMinutes(od.Service.Duration) > dto.Start);

                    if (!isBusy)
                    {
                        result.Add(new AvailableStaffDTO
                        {
                            StaffId = staff.Id,
                            FullName = staff.User.FullName,
                            ImgUrl = staff.ImgURL
                        });
                    }
                }

                return ApiResult<List<AvailableStaffDTO>>.Success(result, "Lấy danh sách nhân viên rảnh thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<AvailableStaffDTO>>.Failure(new Exception("Lỗi khi lấy nhân viên rảnh: " + ex.Message));
            }
        }

        public async Task<ApiResult<List<StaffScheduleRespondDTO>>> GetAllStaffSchedulesAsync()
        {
            try
            {
                var schedules = await _unitOfWork.StaffScheduleRepository
                    .GetAllAsync(null, includes: s => s.Staff.User);

                var result = schedules.Select(s => new StaffScheduleRespondDTO
                {
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm"),
                    Note = s.Note,
                    staffName = s.Staff.User.FullName
                }).OrderBy(s => s.staffName)         // 👉 Sắp xếp theo tên nhân viên (A-Z)
                  .ThenBy(s => s.DayOfWeek).ToList();

                return ApiResult<List<StaffScheduleRespondDTO>>.Success(result, "Lấy toàn bộ lịch làm việc thành công.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<StaffScheduleRespondDTO>>.Failure(new Exception("Lỗi khi lấy danh sách lịch làm việc: " + ex.Message));
            }
        }


        private List<(TimeSpan, TimeSpan)> SubtractRange((TimeSpan Start, TimeSpan End) available, (TimeSpan Start, TimeSpan End) busy)
        {
            var result = new List<(TimeSpan, TimeSpan)>();

            if (busy.End <= available.Start || busy.Start >= available.End)
            {
                result.Add(available);
            }
            else
            {
                if (busy.Start > available.Start)
                    result.Add((available.Start, busy.Start));
                if (busy.End < available.End)
                    result.Add((busy.End, available.End));
            }

            return result;
        }

        private string TimeSpanToString(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }




    }
}
