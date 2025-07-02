using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffScheduleDTO.Respond
{
    public class StaffScheduleRespondDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string? Note { get; set; } // Optional note for the schedule

        public string? staffName { get; set; } // Optional, can be null if not needed
    }

}
