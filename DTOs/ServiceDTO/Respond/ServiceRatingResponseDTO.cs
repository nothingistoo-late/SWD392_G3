using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ServiceDTO.Respond
{
    public class ServiceRatingResponseDTO
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
