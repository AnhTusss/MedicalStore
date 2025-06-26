namespace MedicalStore.Models
{
    public class CheckoutViewModel
    { 
        public Order Order { get; set; } = new Order();
        public List<CartItemSession> CartItems { get; set; } = new List<CartItemSession>();
    }
}

