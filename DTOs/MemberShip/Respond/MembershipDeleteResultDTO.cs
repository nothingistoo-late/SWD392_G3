using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MemberShip.Respond
{
    public class MembershipDeleteResultDTO
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
