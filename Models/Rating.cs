using System.ComponentModel.DataAnnotations;

namespace MedicalStore.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; } // Cho phép null

        [Range(1, 5)]
        public int Star { get; set; }

        public string? Comment { get; set; } // Cho phép null

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
