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
    public class Order : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType Type { get; set; } 
        public OrderStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public string? Notes { get; set; } = null!;
        public ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<OrderMembership>? OrderMemberships { get; set; } = new List<OrderMembership>();


    }
}
