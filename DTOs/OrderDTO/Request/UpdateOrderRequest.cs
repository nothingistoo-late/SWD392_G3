using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Request
{
    public class UpdateOrderRequest
    {
        public Guid OrderId { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
        public List<Guid>? ServiceIds { get; set; } = new();
    }
}
