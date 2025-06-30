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
    }
}
