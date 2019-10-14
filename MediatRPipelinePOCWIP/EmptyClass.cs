using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MediatRPipelinePOCWIP
{
    public class CommandRequest : IRequest<CommandResponse>, IShouldExecuteTask, IShouldMeasure { }

    public class CommandResponse { }

    public class QueryRequest : IRequest<QueryResponse>, IShouldMeasure { }

    public class QueryResponse : IShouldAddAuditLog { }

    public class Request : IRequest<Response> { }

    public class Response : IShouldAddAuditLog { }

    public interface IShouldExecuteTask { }

    public interface IShouldAddAuditLog { }

    public interface IShouldMeasure { }

    public class CommandHandler : IRequestHandler<CommandRequest, CommandResponse>
    {
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(ILogger<CommandHandler> logger)
        {
            _logger = logger;
        }

        Task<CommandResponse> IRequestHandler<CommandRequest, CommandResponse>.Handle(CommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command Handler");
            return Task.FromResult(new CommandResponse());
        }
    }

    public class QueryHandler : IRequestHandler<QueryRequest, QueryResponse>
    {
        private readonly ILogger<QueryHandler> _logger;

        public QueryHandler(ILogger<QueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<QueryResponse> Handle(QueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler");
            return Task.FromResult(new QueryResponse());
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ILogger<Handler> _logger;

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handler");
            return Task.FromResult(new Response());
        }
    }

    public class AllRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public AllRequestPostProcessor(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            _logger.LogInformation("All Request Post Processor");
            return Task.CompletedTask;
        }
    }

    public class CommandRequestPostProcessor : IRequestPostProcessor<IShouldExecuteTask, object>
    {
        private readonly ILogger _logger;

        public CommandRequestPostProcessor(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public Task Process(IShouldExecuteTask request, object response, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Post Process");
            return Task.CompletedTask;
        }
    }

    public class AlsoCommandRequestPostProcessor : IRequestPostProcessor<IShouldExecuteTask, object>
    {
        private readonly ILogger _logger;

        public AlsoCommandRequestPostProcessor(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public Task Process(IShouldExecuteTask request, object response, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Also Task Post Process");
            return Task.CompletedTask;
        }
    }

    public class QueryAuditLogger : IRequestPostProcessor<object, IShouldAddAuditLog>
    {
        private readonly ILogger<Program> _logger;

        public QueryAuditLogger(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public Task Process(object request, IShouldAddAuditLog response, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Query Audit Logger");
            return Task.CompletedTask;
        }
    }

    public class AllPro : IRequestPostProcessor<IShouldMeasure, object>
    {
        private readonly ILogger<Program> _logger;

        public AllPro(ILogger<Program> logger)
        {
            _logger = logger;
        }
        public Task Process(IShouldMeasure request, object response, CancellationToken cancellationToken)
        {
            _logger.LogInformation("All Processing Metric");
            return Task.CompletedTask;
        }
    }
}
