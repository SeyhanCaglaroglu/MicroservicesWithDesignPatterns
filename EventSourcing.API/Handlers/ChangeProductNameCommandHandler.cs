using EventSourcing.API.Commands;
using EventSourcing.API.Services;
using MediatR;

namespace EventSourcing.API.Handlers
{
    public class ChangeProductNameCommandHandler : IRequestHandler<ChangeProductNameCommand>
    {
        private readonly ProductStream _productStream;

        public ChangeProductNameCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(ChangeProductNameCommand request, CancellationToken cancellationToken)
        {
            await _productStream.NameChanged(request.ChangeProductNameDto);
            return Unit.Value;
        }
    }
}
