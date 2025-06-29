using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class StaffSchedule : BaseEntity
    {
        [Key]      
        
        public Guid StaffScheduleId { get; set; }
        [ForeignKey(nameof(Staff))]
        public Guid StaffId { get; set; }
        public virtual Staff Staff { get; set; } = null!;
        // Additional properties or methods can be added as needed
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Note { get; set; }
        // Navigation properties
    }
}
