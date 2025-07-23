using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.VnPay.Request
{
    public class PaymentInformationModel
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class VNPaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class PaymentRequestModel
    {
        public Guid OrderId { get; set; }
    }

    public partial class PaymentModel
    {
        public int Id { get; set; }

        public int PackageId { get; set; }

        public int PostId { get; set; }

        public DateTime PayDate { get; set; }

        public string Description { get; set; } = null!;

        public int TransactionId { get; set; }

        public bool Success { get; set; }

        public string Token { get; set; } = null!;


    }
}
