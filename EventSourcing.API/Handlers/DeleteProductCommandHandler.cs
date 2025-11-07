using EventSourcing.API.Commands;
using EventSourcing.API.Services;
using MediatR;

namespace EventSourcing.API.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly ProductStream _productStream;

        public DeleteProductCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productStream.Deleted(request.Id);
            return Unit.Value;
        }
    }
}
