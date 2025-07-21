using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CustomerMembership.Respond
{
    public class CustomerMembershipResponse
    {
        public Guid Id { get; set; }
        public Guid MembershipId { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive => !EndDate.HasValue || EndDate > DateTime.UtcNow;
    }

}
