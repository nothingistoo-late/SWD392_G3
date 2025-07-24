using BusinessObjects;
using BusinessObjects.Common;
using DTOs.ServiceDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Respond
{
    public class OrderRespondDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailRespondDTO> OrderDetails { get; set; } = new();
    }

    public class OrderDetailRespondDTO
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = default!;
        public OrderDetailStatus Status { get; set; } 
        public string? Note { get; set; } = null!;
        public RatingDTO? Rating { get; set; }
        public DateTime ScheduledTime { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = default!;
    }
    public class RatingDTO
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
    }


}
