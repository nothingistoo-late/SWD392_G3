using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffDTO.Request
{
    public class StaffFilterDTO
    {
        public string? Name { get; set; } // Tìm theo tên gần đúng
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }
    }

}
