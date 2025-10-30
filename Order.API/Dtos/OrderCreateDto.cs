namespace Order.API.Dtos
{
    public class OrderCreateDto
    {
        public string BuyerId { get; set; } = null!;
        public List<OrderItemDto> orderItems { get; set; } = null!;
        public PaymentDto payment { get; set; } = null!;
        public AddressDto Address { get; set; } = null!;
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }

    public class PaymentDto
    {
        public string CardName { get; set; } = null!;
        public string CardNumber { get; set; } = null!; 
        public string Expiration { get; set; } = null!;
        public string CVV { get; set; } = null!;
    }

    public class AddressDto
    {
        public string Line { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
    }
}

