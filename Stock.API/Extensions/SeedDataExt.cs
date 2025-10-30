using Stock.API.Models;

namespace Stock.API.Extensions
{
    public static class SeedDataExt
    {
        public static async Task AddSeedDataExt(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Stocks.AddRange(
                new Models.Stock { ProductId = 1, Count = 100 },
                new Models.Stock { ProductId = 2, Count = 150 }
            );

            await context.SaveChangesAsync();



        }
    }
}
