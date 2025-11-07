using EventSourcing.API.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPut("ChangeName")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeProductNameCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPut("ChangePrice")]
        public async Task<IActionResult> ChangePrice([FromBody] ChangeProductPriceCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {

            await mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);
            return NoContent();
        }
    }
}
