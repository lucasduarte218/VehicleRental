using VehicleRental.Application.Interfaces.Repository;
using VehicleRental.Application.Models;

namespace VehicleRentalAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            IExceptionLogRepository? exceptionLogRepository = context.RequestServices.GetService(typeof(IExceptionLogRepository)) as IExceptionLogRepository;

            ExceptionLog exceptionLog = new ExceptionLog
            {
                OccurredAt = DateTime.UtcNow,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                InnerException = ex.InnerException?.Message,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                IPAddress = context.Connection.RemoteIpAddress?.ToString()
            };

            await exceptionLogRepository.SaveAsync(exceptionLog);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Ocorreu um erro interno. Por favor, tente novamente mais tarde.\"}");
        }
    }

}
