using DTOs.ServiceDTO.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Respond
{
    public class OrderRespondDTO
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public List<ServiceRespondDTO> Services { get; set; } = new();
    }

}
