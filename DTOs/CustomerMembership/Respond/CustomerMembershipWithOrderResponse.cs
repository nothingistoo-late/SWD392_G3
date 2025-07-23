using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CustomerMembership.Respond
{
    public class CustomerMembershipWithOrderResponse
    {
        public CustomerMembershipResponse Membership { get; set; } = null!;
        public OrderResponse Order { get; set; } = null!;
    }
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public double TotalPrice { get; set; }
        public string? Notes { get; set; }
    }
}
