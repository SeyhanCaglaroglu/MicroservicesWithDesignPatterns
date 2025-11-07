namespace EventSourcing.API.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
