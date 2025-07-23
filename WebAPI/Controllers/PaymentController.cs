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
        private readonly ICurrentUserService _currentUserService;
        private readonly IMembershipService _membershipService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;


        public PaymentController(ICustomerService customerService ,IUnitOfWork unit ,IMembershipService membershipService ,ICurrentUserService currentUserService ,ICustomerMembershipService customerMembershipService, IOrderService order ,VnPayService vpnPayService)
        {
            _vpnPayService = vpnPayService;
            _orderService = order;
            _customerMemberShipService = customerMembershipService;
            _currentUserService = currentUserService;
            _membershipService = membershipService;
            _unitOfWork = unit;
            _customerService = customerService;
        }

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
                Amount = amout,
                OrderType = request.OrderType.ToString()
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
                var orderInfo = Request.Query["vnp_OrderInfo"];
                var parts = orderInfo.ToString().Split(' ');

                var orderId = Guid.Parse(parts[0]);
                var orderType = parts.Length > 1 ? Guid.Parse(parts[1]) : Guid.Empty;

                var order = await _orderService.GetOrderByIdAsync(orderId);
                foreach (var item in Request.Query)
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                }

                if (!order.IsSuccess)
                {
                    return NotFound("Order not found");
                }

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

                    if (orderType!=Guid.Empty)
                    {
                       var result =  await _customerMemberShipService.CreateMembershipOrderForCustomerAsync(order.Data.CustomerId, orderType);
                        if (!result.IsSuccess)
                        {
                            return BadRequest(result);
                        }
                    }

                    return Ok("Ditme mình thành tỷ phú rồi đại vương ơi!!");
                }

                //return Ok(response);
                return Ok("Ditme tiền giả rồi đại vương ơi!!");
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework)
                return BadRequest("An error occurred while processing the payment: "+ex.Message);
            }
        }

    }
}
