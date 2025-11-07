namespace EventSourcing.API.Dtos
{
    public class CreateProductDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
