using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Models;
using Newtonsoft.Json;
using System.Threading;

namespace ApiApplication.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {        
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {           
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);               
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken = default)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            CustomProblemDetails problem = new CustomProblemDetails();          

            switch (ex)
            {
                case BadRequestException BadRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = BadRequestException.Message,
                        Status = (int)statusCode,
                        Detail = BadRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = BadRequestException.ValidationErrors
                    };
                    break;
                case NotFoundException NotFound:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = NotFound.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = NotFound.InnerException?.Message,
                    };
                    break;
                case InvalidUserOrPassword InvalidUserOrPassword:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = InvalidUserOrPassword.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = InvalidUserOrPassword.InnerException?.Message,
                    };
                    break;
                case ReservationAlreadyPaidException ReservationAlreadyPaidException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = ReservationAlreadyPaidException.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = ReservationAlreadyPaidException.InnerException?.Message,
                    };
                    break;
                case InvalidAuditoriumException InvalidAuditoriumException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = InvalidAuditoriumException.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                        Detail = InvalidAuditoriumException.InnerException?.Message,
                    };
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    problem = new CustomProblemDetails
                    {
                        Title = ex.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.InternalServerError),
                        Detail = ex.StackTrace,
                    };
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            var logMessage = JsonConvert.SerializeObject(problem);
            _logger.LogError(logMessage);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(logMessage, cancellationToken);
        }
    }
}
