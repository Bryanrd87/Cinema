using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiApplication.Middleware
{
    public class ExecutionTimeMiddleware : IMiddleware
    {       
        private readonly ILogger<ExecutionTimeMiddleware> _logger;
        public ExecutionTimeMiddleware(ILogger<ExecutionTimeMiddleware> logger)
        {           
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await next(context);

            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;

           _logger.LogInformation($"{context.Request.Method} {context.Request.Path} executed in {executionTime} ms");
        }       
    }
}
