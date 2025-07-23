using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDTO.Request
{
    public class UpdateOrderStatusRequestDTO
    {
        [Required(ErrorMessage = "Order Status is required.")]
        public OrderStatus Status { get; set; }
    }
}
