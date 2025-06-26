namespace MedicalStore.Models
{
    public class Article
    {
        public int Id { get; set; }                  
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Content { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
