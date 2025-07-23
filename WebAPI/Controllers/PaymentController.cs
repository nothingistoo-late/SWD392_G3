using BusinessObjects.Common;
using DTOs.OrderDTO.Request;
using DTOs.VnPay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391.KoiCareSystemAtHome.Service.Services;

namespace SWP391.KoiCareSystemAtHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly VnPayService _vpnPayService;
        private readonly IOrderService _orderService;
        private readonly ICustomerMembershipService _customerMemberShipService;


        public PaymentController(ICustomerMembershipService customerMembershipService, IOrderService order ,VnPayService vpnPayService)
        {
            _vpnPayService = vpnPayService;
            _orderService = order;
            _customerMemberShipService = customerMembershipService;
        }

        //[HttpGet("getPaymentByAdvId/{advId}")]
        //public async Task<ActionResult<IEnumerable<VNPaymentResponseModel>>> GetPaymentByAdvId(int advId)
        //{
        //    var payments = await _paymentService.GetPaymentByAdvIdAsync(advId);

        //    if (payments == null || !payments.Any())
        //        return NotFound();

        //    var response = payments.Select(x => new PaymentResponseModel
        //    {
        //        Id = x.Id,
        //        PackageId = x.PackageId,
        //        PostId = x.PostId,
        //        PayDate = x.PayDate,
        //        Description = x.Description,
        //        TransactionId = x.TransactionId,
        //        Success = x.Success,
        //        Token = x.Token,
        //    });

        //    return Ok(response);
        //}

        //[HttpGet("getPaymentByPacKageId/{packageId}")]
        //public async Task<ActionResult<IEnumerable<PaymentResponseModel>>> GetPaymentByPackageId(int packageId)
        //{
        //    var payments = await _paymentService.GetPaymentByPackageIdAsync(packageId);

        //    if (payments == null || !payments.Any())
        //        return NotFound();

        //    var response = payments.Select(x => new PaymentResponseModel
        //    {
        //        Id = x.Id,
        //        PackageId = x.PackageId,
        //        PostId = x.PostId,
        //        PayDate = x.PayDate,
        //        Description = x.Description,
        //        TransactionId = x.TransactionId,
        //        Success = x.Success,
        //        Token = x.Token,
        //    });

        //    return Ok(response);
        //}

        //[HttpGet("getPaymentByPaymentId/{paymentId}")]
        //public async Task<ActionResult<IEnumerable<PaymentResponseModel>>> GetPaymentByPaymentId(int paymentId)
        //{
        //    var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

        //    if (payment == null)
        //        return NotFound();

        //    var response = new PaymentResponseModel
        //    {
        //        Id = payment.Id,
        //        PackageId = payment.PackageId,
        //        PostId = payment.PostId,
        //        PayDate = payment.PayDate,
        //        Description = payment.Description,
        //        TransactionId = payment.TransactionId,
        //        Success = payment.Success,
        //        Token = payment.Token,
        //    };

        //    return Ok(response);
        //}

        [HttpPost("createPaymentURL")]
        public async Task<ActionResult> CreatePaymentURL(PaymentRequestModel request)
        {

            var order = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (!order.IsSuccess)
            {
                return NotFound("Order not found");
            }

            var customerMembership = await _customerMemberShipService.GetBestActiveMembershipByCustomerAsync(order.Data.CustomerId);
            decimal amout = 0;
            if (!customerMembership.IsSuccess)
                amout = order.Data.TotalPrice;
            else
                amout = order.Data.TotalPrice * (1 - customerMembership.Data.DiscountPercentage / 100m);


            PaymentInformationModel model = new()
            {
                OrderId = order.Data.Id,
                Amount = amout 
            };

            string url = _vpnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(url);
        }

        [HttpGet("paymentCallBack")]
        public async Task<ActionResult> PaymentCallBack()
        {
            if (!Request.Query.Any())
            {
                return BadRequest("Missing query parameters");
            }

            try
            {
                var response = _vpnPayService.PaymentExecute(Request.Query);

                //string[] splitDescription = response.OrderDescription.Split(" ");

                //var paymentModel = new PaymentModel
                //{
                //    PackageId = int.Parse(splitDescription[1]),
                //    PostId = int.Parse(splitDescription[0]),
                //    Description = response.OrderDescription,
                //    PayDate = DateTime.Now,
                //    TransactionId = int.Parse(response.TransactionId),
                //    Success = response.Success,
                //    Token = response.Token,
                //};

                //var advModel = new UpdateAdsModel
                //{
                //    PackageId = int.Parse(splitDescription[1]),
                //    PostId = int.Parse(splitDescription[0]),
                //    PayDate = DateTime.Now,
                //};

                //bool success = false;

                //Console.WriteLine(paymentModel.PackageId + " " + paymentModel.PostId );
                var vnpTxnRef = Request.Query["vnp_OrderInfo"];

                var order = await _orderService.GetOrderByIdAsync(Guid.Parse(vnpTxnRef));

                if (response.Success && response.TransactionId != "0")
                {
                    //success = await _paymentService.CreatePaymentAsync(paymentModel);
                    //await _paymentService.CreatePaymentAsync(paymentModel);
                    //await _advService.UpdateAdsPaymentAsync(advModel);
                    var status = new UpdateOrderStatusRequestDTO
                    {
                        Status = OrderStatus.Paid
                    };

                    await _orderService.UpdateOrderStatusAsync(order.Data.Id, status);

                    //_orderService.UpdateOrderAsync

                    return Ok("Ditme mình thành tỷ phú rồi đại vương ơi!!");
                }

                //return Ok(response);
                return Ok("Ditme tiền giả rồi đại vương ơi!!");
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework)
                return StatusCode(500, "An error occurred while processing the payment");
            }
        }

    }
}
