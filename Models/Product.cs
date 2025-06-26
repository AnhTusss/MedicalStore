namespace MedicalStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = "/images/default.jpg";

        public int CategoryId { get; set; }

        public Category? Category { get; set; } 
    }
}
