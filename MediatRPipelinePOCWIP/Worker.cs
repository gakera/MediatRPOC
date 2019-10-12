using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediatRPipelinePOCWIP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMediator _mediator;

        public Worker(ILogger<Worker> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _mediator.Send(new CommandRequest());
                await Task.Delay(1000, stoppingToken);
                await _mediator.Send(new QueryRequest());
                await Task.Delay(1000, stoppingToken);
                await _mediator.Send(new Request());
                await Task.Delay(100000, stoppingToken);
            }
        }
    }
}
