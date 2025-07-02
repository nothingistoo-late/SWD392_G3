using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffDTO.Respond
{
    public class StaffRespondDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public double Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string? ImgURL { get; set; } = string.Empty;
        public string? Note { get; set; } // 👈 NEW


    }
}
