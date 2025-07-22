using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.VnPay.Request
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }

}
