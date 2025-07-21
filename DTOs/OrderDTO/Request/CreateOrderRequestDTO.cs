using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Request
{
    public class CreateOrderRequestDTO
    {

        [Required(ErrorMessage = "Cần phải nhập customer id ")]
        public Guid CustomerId { get; set; }
        [Required(ErrorMessage = "Cần phải nhập ngày order")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string? note { get; set; }

        [Required(ErrorMessage = "Cần phải nhập ít nhất 1 service id")]
        public List<OrderServiceItemDTO> Services { get; set; }
    }
    public class OrderServiceItemDTO
    {
        public Guid ServiceId { get; set; }
        public DateTime ScheduledTime { get; set; }
    }

}
