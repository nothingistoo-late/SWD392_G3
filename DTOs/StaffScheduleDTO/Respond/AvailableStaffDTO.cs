using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffScheduleDTO.Respond
{
    public class AvailableStaffDTO
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; } = null!;
        public string? ImgUrl { get; set; }
    }

}
