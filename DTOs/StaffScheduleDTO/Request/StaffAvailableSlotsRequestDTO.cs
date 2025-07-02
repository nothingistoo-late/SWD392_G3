using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffScheduleDTO.Request
{
    public class    StaffAvailableSlotsRequestDTO
    {
        public Guid StaffId { get; set; }
        public DateTime Date { get; set; } // Ngày muốn kiểm tra (yyyy-MM-dd)
    }

}
