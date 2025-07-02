using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffScheduleDTO.Request
{
    public class StaffScheduleFilterDTO
    {
        public Guid? StaffId { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
    }

}
