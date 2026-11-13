using System.Reflection;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Use(async (context, next) => {
    // Only application traffic contributes to the drain log. Health probes must not prevent deployment.
    if (context.Request.Path != "/health") {
        var log = Environment.GetEnvironmentVariable("TRAFFIC_LOG") ?? "/var/log/emes/traffic.log";
        await File.AppendAllTextAsync(log, $"{DateTime.UtcNow:O} {context.Request.Path} started\n");
        await next();
        await File.AppendAllTextAsync(log, $"{DateTime.UtcNow:O} {context.Request.Path} finished\n");
    } else await next();
});
app.MapGet("/health", () => Results.Ok(new { status = "ready" }));
app.MapGet("/", () => new { node = Environment.MachineName, version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() });
app.MapGet("/slow", async () => { await Task.Delay(TimeSpan.FromSeconds(10)); return Results.Ok("finished"); });
app.Run();
public partial class Program { }
