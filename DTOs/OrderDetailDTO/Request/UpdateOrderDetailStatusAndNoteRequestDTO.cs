using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.OrderDetailDTO.Request
{
    public class UpdateOrderDetailStatusAndNoteRequestDTO
	{
		public Guid OrderDetailId { get; set; }
		public OrderDetailStatus NewStatus { get; set; }
		public string? Note { get; set; }
	}

}
