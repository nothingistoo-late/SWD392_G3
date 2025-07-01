using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IStaffScheduleRepository : IGenericRepository<StaffSchedule, Guid>
    {
        Task<List<Staff>> GetAvailableStaffs(DateTime start, DateTime end);
        Task<bool> IsWithinWorkingHours(Guid staffId, DateTime start, DateTime end);

    }
}
