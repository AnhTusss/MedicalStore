namespace MedicalStore.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Order { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string Product { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
