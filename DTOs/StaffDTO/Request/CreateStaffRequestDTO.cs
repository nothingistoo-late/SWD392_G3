using BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.StaffDTO.Request
{
    public class CreateStaffRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "FirstName là bắt buộc")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName là bắt buộc")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "giới tính là bắt buộc")]
        public string Gender { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương thì không được âm")]
        public double Salary { get; set; }

        [DataType(DataType.Date)]
        [CustomHireDateValidation(ErrorMessage = "Ngày thuê không được là ở tương lai")]
        public DateTime HireDate { get; set; }

        public string? ImgURL { get; set; }

        public string? Note { get; set; }
    }

    public class CustomHireDateValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Now;
            }
            return false;
        }
    }


}
