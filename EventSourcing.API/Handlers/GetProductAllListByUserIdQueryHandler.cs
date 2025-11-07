using EventSourcing.API.Dtos;
using EventSourcing.API.Models;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.Handlers
{
    public class GetProductAllListByUserIdQueryHandler(AppDbContext context) : IRequestHandler<GetProductAllListByUserIdQuery, List<ProductDto>>
    {
        public async Task<List<ProductDto>> Handle(GetProductAllListByUserIdQuery request, CancellationToken cancellationToken)
        {
            var products = await context.Products.Where(p => p.UserId == request.UserId).ToListAsync(cancellationToken);

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
        }
    }
}
