using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class CustomerMembership
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        public Guid MembershipId { get; set; }
        public Membership Membership { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public int DurationInDays { get; set; } // 👈 Thêm chỗ này

        public DateTime? EndDate { get; set; } // null nếu đang dùng
    }

}
