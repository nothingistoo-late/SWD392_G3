using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class OrderMembership : BaseEntity 
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Order))]

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        [ForeignKey(nameof(Membership))]
        public Guid MembershipId { get; set; }
        public virtual Membership Membership { get; set; } = null!;


    }
}
