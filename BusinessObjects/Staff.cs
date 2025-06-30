using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessObjects
{
    public class Staff : BaseEntity
    {
        [Key, ForeignKey(nameof(User))]
        public Guid Id { get; set; }
        public virtual User User { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public double Salary { get; set; }
        public DateTime HireDate { get; set; }
        public virtual ICollection<StaffSchedule> StaffSchedules { get; set; } 
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } 
    }
}
