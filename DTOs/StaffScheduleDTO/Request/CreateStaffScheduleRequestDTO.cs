using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffScheduleDTO.Request
{
    public class CreateStaffScheduleRequestDTO
    {
        public Guid StaffId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Note { get; set; } = string.Empty; // Optional note for the schedule
    }

}
