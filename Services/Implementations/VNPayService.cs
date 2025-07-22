using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class VnPaySettings
{
    public string TmnCode { get; set; }
    public string HashSecret { get; set; }
    public string PayUrl { get; set; }
    public string ReturnUrl { get; set; }
    public string CallbackUrl { get; set; }
}

public class VnPayService
{
    private readonly VnPaySettings _settings;

    public VnPayService(IOptions<VnPaySettings> settings)
    {
        _settings = settings.Value;
    }

    public string CreatePaymentUrl(string orderId, decimal amount, string orderInfo)
    {
        var timeNow = DateTime.Now;
        var tick = timeNow.Ticks.ToString();
        var amountInVND = ((int)amount * 100).ToString(); // VNPAY tính theo VND x100

        var vnp_Params = new SortedDictionary<string, string>
        {
            {"vnp_Version", "2.1.0"},
            {"vnp_Command", "pay"},
            {"vnp_TmnCode", _settings.TmnCode},
            {"vnp_Amount", amountInVND},
            {"vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss")},
            {"vnp_CurrCode", "VND"},
            {"vnp_IpAddr", "127.0.0.1"},
            {"vnp_Locale", "vn"},
            {"vnp_OrderInfo", orderInfo},
            {"vnp_OrderType", "billpayment"},
            {"vnp_ReturnUrl", _settings.ReturnUrl},
            {"vnp_IpnUrl", _settings.CallbackUrl}, // ✅ Cái dòng QUAN TRỌNG NHẤT
            {"vnp_TxnRef", orderId}
        };

        string signData = string.Join('&', vnp_Params.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
        string sign = HmacSHA512(_settings.HashSecret, signData);
        vnp_Params["vnp_SecureHash"] = sign;

        string queryString = string.Join("&", vnp_Params.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
        return $"{_settings.PayUrl}?{queryString}";
    }

    public bool ValidateCallback(IQueryCollection query, out string txnRef, out string vnpResponseCode)
    {
        var vnpData = query
            .Where(x => x.Key.StartsWith("vnp_") && x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        var signData = string.Join('&', vnpData.OrderBy(x => x.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        var secureHash = query["vnp_SecureHash"];
        var hash = HmacSHA512(_settings.HashSecret, signData);

        txnRef = query["vnp_TxnRef"];
        vnpResponseCode = query["vnp_ResponseCode"];
        return secureHash == hash;
    }

    private string HmacSHA512(string key, string input)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(input);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
