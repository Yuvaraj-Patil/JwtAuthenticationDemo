using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace JwtAuthenticationDemo.CustomMiddleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly string _logFilePath;

    public LoggingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;

        _logFilePath = _configuration["LoggingMiddleware:LogFilePath"] ?? "C:\\Logs\\log.txt";
        EnsureLogDirectoryExists(_logFilePath);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();        
        string logEntry;

        try
        {
            await _next(context);
            stopwatch.Stop();

            logEntry = $"[Log Entry Time: {DateTime.Now}] | Elapsed Time: {stopwatch.ElapsedMilliseconds}ms | StatusCode: {context.Response.StatusCode} | StatusMessage: {Enum.GetName(typeof(HttpStatusCode), context.Response.StatusCode)}";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logEntry = $"[Log Entry Time: {DateTime.Now}] | Elapsed Time: {stopwatch.ElapsedMilliseconds}ms | StatusCode: 500 | Exception Details: {ex.Message}\n{ex.StackTrace}";
            await context.Response.WriteAsync("An unexpected error occurred.");
        }

        AppendToLogFile(_logFilePath, logEntry);
    }

    private void EnsureLogDirectoryExists(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void AppendToLogFile(string filePath, string logEntry)
    {
        try
        {
            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLine(logEntry);
            }
        }
        catch
        {
            // Silent fail — or log to system fallback
        }
    }
}
