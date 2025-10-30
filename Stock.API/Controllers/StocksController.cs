using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock.API.Models;

namespace Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            return Ok(await context.Stocks.ToListAsync());
        }
    }
}
