using MediatR;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Data.Cqs.Commands.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.CommandsHandlers.Hangfire
{
    public class HangfireInitCommandHandler : IRequestHandler<HangfireInitCommand>
    {
        private readonly IHangfireService _hangfireService;

        public HangfireInitCommandHandler(IHangfireService hangfireService)
        {
            _hangfireService = hangfireService;
        }

        public async Task Handle(HangfireInitCommand request, CancellationToken cancellationToken)
        {
            await _hangfireService.Init();
        }
    }
}
