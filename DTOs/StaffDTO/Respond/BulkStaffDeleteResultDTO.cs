using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffDTO.Respond
{
    public class BulkStaffDeleteResultDTO
    {
        public List<StaffRespondDTO> Deleted { get; set; } = new();
        public List<FailedStaffDeleteDTO> Failed { get; set; } = new();
    }

    public class FailedStaffDeleteDTO
    {
        public Guid StaffId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}
