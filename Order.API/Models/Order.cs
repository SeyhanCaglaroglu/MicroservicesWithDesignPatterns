namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BuyerId { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; }
        public string? FailMessage { get; set; }
    }

    public enum OrderStatus
    {
        Suspend,
        Complete,
        Fail
    }
}
