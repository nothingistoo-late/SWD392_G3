using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ServiceDTO.Request
{
    public class UpdateServiceRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public int? Price { get; set; }
        public string? Description { get; set; } = null!;
        public int? Duration { get; set; } // Duration in minutes
    }
}
