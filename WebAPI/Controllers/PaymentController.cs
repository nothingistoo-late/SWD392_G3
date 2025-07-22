using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[Route("api/[controller]")]
[ApiController]
public class VnPayController : ControllerBase
{
    private readonly VnPayService _vnPayService;
    private readonly VnPaySettings _vnPaySettings;

    public VnPayController(IOptions<VnPaySettings> vnPayOptions, VnPayService vnPayService)
    {
        _vnPayService = vnPayService;
        _vnPaySettings = vnPayOptions.Value;

    }

    [HttpGet("create")]
    public IActionResult CreatePaymentUrl(string orderId, decimal amount, string orderInfo)
    {
        var url = _vnPayService.CreatePaymentUrl(orderId, amount, orderInfo);
        return Ok("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" +url );
    }

    [HttpGet("callback")]
    public IActionResult PaymentCallback()
    {
        if (_vnPayService.ValidateCallback(Request.Query, out string orderId, out string code))
        {
            if (code == "00")
            {
                // Thanh toán thành công
                // TODO: Update OrderStatus = "Paid" trong DB
                return Ok("IPN thanh toán thành công");
            }
            else
            {
                // Thanh toán thất bại
                return Ok("IPN thanh toán thất bại");
            }
        }

        return BadRequest("Invalid signature");
    }

    [HttpGet("return")]
    public IActionResult PaymentReturn()
    {
        if (_vnPayService.ValidateCallback(Request.Query, out string orderId, out string code))
        {
            if (code == "00")
                return Redirect("facebook.com/hctrung2k4");
            else
                return Redirect("facebook.com/hctrung2k4");
        }

        return Content("Xác minh thất bại");
    }
}
