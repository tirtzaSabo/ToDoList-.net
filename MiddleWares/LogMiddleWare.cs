using System.Diagnostics;
using MyTask.Models;

namespace MyTask.MiddleWares;

public class LogMiddleware
{
    private RequestDelegate next;
    private readonly string logFilePath;

    public LogMiddleware(RequestDelegate next, string logFilePath)
    {
        this.next = next;
        this.logFilePath = logFilePath;
    }

    public async Task Invoke(HttpContext c)
    {
        var sw = new Stopwatch();
        sw.Start();
        await next(c);
        WriteLogToFile(
            $"{c.Request.Path}.{c.Request.Method} time:{DateTime.Now} took {sw.ElapsedMilliseconds}ms."
                + $" User: {c.User?.FindFirst("Id")?.Value ?? "unknown"}"
        );
    }

    private void WriteLogToFile(string logMessage)
    {
        using (StreamWriter sw = File.AppendText(logFilePath))
        {
            sw.WriteLine(logMessage);
        }
    }
}

public static partial class OurMiddleExtensions
{
    public static IApplicationBuilder UseLogMiddleware(
        this IApplicationBuilder builder,
        string logFilePath
    )
    {
        return builder.UseMiddleware<LogMiddleware>(logFilePath);
    }
}