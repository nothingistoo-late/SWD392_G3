using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Rating : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(OrderDetail))]
        public Guid OrderDetailId { get; set; }

        public int Score { get; set; } // ví dụ: 1 → 5
        public string? Comment { get; set; }
        public virtual OrderDetail OrderDetail { get; set; } = default!;
    }
}
