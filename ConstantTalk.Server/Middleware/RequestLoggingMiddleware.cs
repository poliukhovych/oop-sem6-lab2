using ConstantTalk.Server.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System.Text;

namespace ConstantTalk.Server.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly RequestLoggingSettings _settings;
        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger,
            IOptions<RequestLoggingSettings> options)
        {
            _next = next;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            if (_settings.ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var request = context.Request;
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var requestBodyText = await ReadRequestBodyAsync(request);
            string responseBodyText = "";
            int statusCode = 0;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
                
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                responseBody.Seek(0, SeekOrigin.Begin);
                responseBodyText = await ReadResponseBodyAsync(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RequestLoggingMiddleware");
                throw;
            }
            finally
            {
                stopwatch.Stop();

                var logEntry = new StringBuilder();
                logEntry.AppendLine("=== HTTP REQUEST ===");
                logEntry.AppendLine($"Time: {DateTime.Now}");
                logEntry.AppendLine($"Method: {request.Method}");
                logEntry.AppendLine($"Path: {request.Path}");
                logEntry.AppendLine($"Status Code: {statusCode}");
                logEntry.AppendLine($"Request Body: {requestBodyText}");
                logEntry.AppendLine($"Response Body: {responseBodyText}");
                logEntry.AppendLine($"Time of execution: {stopwatch.ElapsedMilliseconds} ms");
                logEntry.AppendLine("==================\n");

                _logger.LogInformation(logEntry.ToString());
                await WriteLogToFileAsync(logEntry.ToString());

                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                leaveOpen: true
            );
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return string.IsNullOrWhiteSpace(body) ? "(empty)" : body;
        }

        private async Task<string> ReadResponseBodyAsync(Stream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBody, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            return string.IsNullOrWhiteSpace(body) ? "(empty)" : body;
        }

        private async Task WriteLogToFileAsync(string logEntry)
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(_settings.LogFilePath);
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory!);

                await File.AppendAllTextAsync(_settings.LogFilePath, logEntry, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error writing log to the file: {ex.Message}");
            }
        }
    }
}
