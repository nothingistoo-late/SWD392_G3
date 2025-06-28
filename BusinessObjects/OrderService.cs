using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class OrderService : BaseEntity
    {
        [Key]
        public int OrderServiceId { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;


    }
}
