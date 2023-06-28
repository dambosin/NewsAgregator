using AutoMapper;
using MediatR;
using NewsAgregator.Data.Cqs.Commands.Token;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Data.Cqs.CommandsHandlers
{
    public class AddRefreshTokenCommandHandler : IRequestHandler<AddRefreshTokenCommand>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IMapper _mapper;
        public AddRefreshTokenCommandHandler(NewsAgregatorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task Handle(AddRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Add(_mapper.Map<RefreshToken>(request));
            return Task.CompletedTask;
        }
    }
}