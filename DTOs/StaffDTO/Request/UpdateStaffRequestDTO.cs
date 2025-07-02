using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffDTO.Request
{
    public class UpdateStaffRequestDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public double? Salary { get; set; }
        public DateTime? HireDate { get; set; }
        public string? ImgURL { get; set; }
        public string? Note { get; set; }
    }

}
