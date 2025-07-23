using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Request
{
    public class CreateMembershipPaymentRequest
    {
        public Guid CustomerId { get; set; }
        public Guid MembershipId { get; set; }
    }
}
