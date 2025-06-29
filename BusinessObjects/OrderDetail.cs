using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class OrderDetail : BaseEntity
    {
        [Key]
        public Guid OrderDetailId { get; set; }

        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; } = null!;

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;


    }
}
