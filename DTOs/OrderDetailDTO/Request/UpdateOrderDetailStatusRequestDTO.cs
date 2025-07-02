using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetailDTO.Request
{
    public class UpdateOrderDetailStatusRequestDTO
    {
        public OrderDetailStatus NewStatus { get; set; }
    }

}
