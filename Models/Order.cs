namespace MedicalStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public string? Status { get; set; } = "Chờ xác nhận";

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
