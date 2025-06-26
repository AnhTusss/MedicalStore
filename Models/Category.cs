using System.Collections.Generic;

namespace MedicalStore.Models
{
    public class Category
    {
        public Category()
        {
            Name = string.Empty;
            ImageUrl = string.Empty; // Khởi tạo thêm
            Products = new List<Product>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; } 

        public List<Product> Products { get; set; }
    }
}
