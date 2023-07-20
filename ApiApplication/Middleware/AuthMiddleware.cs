using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace ApiApplication.Middleware
{
    public class AuthMiddleware : IMiddleware
    {        
        private readonly ILogger<AuthMiddleware> _logger;
        public AuthMiddleware(ILogger<AuthMiddleware> logger)
        {           
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            await next(httpContext);

            if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized) 
            {
                _logger.LogWarning("Unauthorized: Access denied");

                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                httpContext.Response.ContentType = "text/plain";
               
                await httpContext.Response.WriteAsync("Unauthorized: Access denied");
            }
        }      
    }
}
