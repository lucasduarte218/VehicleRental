using System.Diagnostics;
using VehicleRental.Application.Interfaces.Repository;
using VehicleRental.Application.Models;

namespace VehicleRentalAPI.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            context.Request.EnableBuffering();
            string requestBody = string.Empty;
            if (context.Request.ContentLength.GetValueOrDefault() > 0)
            {
                using (StreamReader reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
            }

            DateTime requestDate = DateTime.UtcNow;
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            string requestMethod = context.Request.Method;
            string? ipAddress = context.Connection.RemoteIpAddress?.ToString();

            Stream originalBodyStream = context.Response.Body;
            using MemoryStream responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            DateTime responseDate = DateTime.UtcNow;
            stopwatch.Stop();
            long executionTime = stopwatch.ElapsedMilliseconds;

            ILogRepository? logRepository = context.RequestServices.GetService(typeof(ILogRepository)) as ILogRepository;

            Log log = new Log
            {
                UserAgent = userAgent,
                RequestMethod = requestMethod,
                IPAddress = ipAddress,
                RequestDate = requestDate,
                SerializedRequest = requestBody,
                ResponseDate = responseDate,
                SerializedResponse = responseBody,
                ResponseStatusCode = context.Response.StatusCode,
                TotalExecutionTime = executionTime
            };

            await logRepository.SaveAsync(log);

            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
    }

}
