using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.VnPay.Request
{
    public class GenerateVietQRRequestDTO
    {
        public int Amount { get; set; }
        public string AddInfo { get; set; } = null!;
    }

}
