using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Membership : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; } // 💥 Giảm bao nhiêu % khi mua hàng
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DurationInDays { get; set; } // Số ngày có hiệu lực
        public ICollection<CustomerMembership> CustomerMemberships { get; set; } = new List<CustomerMembership>();
    }
}
