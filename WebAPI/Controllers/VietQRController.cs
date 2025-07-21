using DTOs.VnPay.Request;
using DTOs.VnPay.Respond;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VietQRController : Controller
    {
        private readonly IVietQRService _vietQRService;

        public VietQRController(IVietQRService vietQRService)
        {
            _vietQRService = vietQRService;
        }

        [HttpPost("GenerateQR")]
        public IActionResult GenerateQr([FromBody] GenerateVietQRRequestDTO request)
        {
            if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.AddInfo))
            {
                return BadRequest("Số tiền hoặc nội dung không hợp lệ.");
            }

            string url = _vietQRService.GenerateQrUrl(request.Amount, request.AddInfo);

            return Ok(new GenerateVietQRResponseDTO
            {
                QrImageUrl = url
            });
        }
    }
}
