using Microsoft.EntityFrameworkCore;
using Repositories.WorkSeeds.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class StaffScheduleRepository : GenericRepository<StaffSchedule, Guid>, IStaffScheduleRepository
    {
        public StaffScheduleRepository(SWD392_G3DBcontext context) : base(context)
        {
        }

        public async Task<List<Staff>> GetAvailableStaffs(DateTime start, DateTime end)
        {
            var day = start.DayOfWeek;
            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            var schedules = await _context.StaffSchedules
                .Include(s => s.Staff)
                .Where(s =>
                    s.DayOfWeek == day &&
                    s.StartTime <= startTime &&
                    s.EndTime >= endTime &&
                    !s.IsDeleted &&
                    !s.Staff.IsDeleted // 👈 Nếu Staff cũng có IsDeleted
                )
                .Select(s => s.Staff)
                .Distinct()
                .ToListAsync();

            return schedules;
        }
    }
}
