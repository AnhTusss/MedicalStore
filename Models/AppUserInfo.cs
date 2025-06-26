using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalStore.Models
{
    public class AppUserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; } 

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; } 

        [StringLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}   
