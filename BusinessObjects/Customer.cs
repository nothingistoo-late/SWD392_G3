using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Customer : BaseEntity
    {
        [Key, ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string imgURL { get; set; } = null!;
        public Membership Membership { get; set; }
        public virtual ICollection<Order>? Orders { get; set; } 

    }
}
