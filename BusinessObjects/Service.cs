using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Service : BaseEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public int? Price { get; set; }
        public string Description { get; set; } = null!;
        public int Duration { get; set; } // Duration in minutes
    }
}
