using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MemberShip.Request
{
    // MembershipDTOs/CreateMembershipRequest.cs
    public class CreateMembershipRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public double DiscountRate { get; set; } // ví dụ 0.1 là 10%
    }


}
