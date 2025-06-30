using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ServiceDTO.Request
{
    public class AddServiceRequestDTO
    {
        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Giá dịch vụ là bắt buộc")]

        public int? Price { get; set; }
        [Required(ErrorMessage = "Mô tả dịch vụ là bắt buộc")]

        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "Thời gian dịch vụ là bắt buộc")]
        public int Duration { get; set; } // Duration in minutes
    }
}
