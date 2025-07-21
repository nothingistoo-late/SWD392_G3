using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MemberShip.Request
{
    public class UpdateMembershipRequest
    {
        public string? Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; } // 💥 Giảm bao nhiêu % khi mua hàng
        public string? Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public int? DurationInDays { get; set; }
    }
}
