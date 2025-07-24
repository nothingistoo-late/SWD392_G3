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

        private string UIurl = "http://localhost:3000";
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

                if (!order.IsSuccess)
                {
                    return Redirect($"{UIurl}/order-success?orderId={orderId}&success=false");
                }

                if (response.Success && response.TransactionId != "0")
                {
                    var status = new UpdateOrderStatusRequestDTO
                    {
                        Status = OrderStatus.Paid
                    };

                    await _orderService.UpdateOrderStatusAsync(order.Data.Id, status);

                    if (orderType != Guid.Empty)
                    {
                        var result = await _customerMemberShipService.CreateMembershipOrderForCustomerAsync(order.Data.CustomerId, orderType, orderId);
                        if (!result.IsSuccess)
                        {
                            return Redirect($"{UIurl}/membership-success?orderId={orderId}&success=false");
                        }
                        else
                        {
                            return Redirect($"{UIurl}/membership-success?orderId={orderId}&success=true");
                        }
                    }

                    return Redirect($"{UIurl}/order-success?orderId={orderId}&success=true");
                }

                return Redirect($"{UIurl}/order-success?orderId={orderId}&success=false");
            }
            catch (Exception ex)
            {
                return Redirect($"{UIurl}/order-success?orderId=unknown&success=false&message={Uri.EscapeDataString(ex.Message)}");
            }
        }

    }
}
