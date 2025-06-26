using System.ComponentModel.DataAnnotations;

namespace MedicalStore.Models
{
    public class RoleAssignment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
