using System.ComponentModel.DataAnnotations;

namespace MedicalStore.Models
{
    public class UpdateProfileModel
    {
        [Display(Name = "Tên tài khoản")]
        public string? Username { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Họ tên")]
        public string? FullName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
    }
}
