using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class VietQRService : IVietQRService
    {
        private const string BankCode = "MB";
        private const string AccountNumber = "0339381305";
        private const string AccountName = "HOANG CHI TRUNG";

        public string GenerateQrUrl(int amount, string addInfo)
        {
            string baseUrl = "https://img.vietqr.io/image";
            string qrPath = $"{BankCode}-{AccountNumber}-compact.png";
            string encodedAddInfo = Uri.EscapeDataString(addInfo);
            string encodedAccountName = Uri.EscapeDataString(AccountName);
            return $"{baseUrl}/{qrPath}?amount={amount}&addInfo={encodedAddInfo}&accountName={encodedAccountName}";
        }
    }
}
