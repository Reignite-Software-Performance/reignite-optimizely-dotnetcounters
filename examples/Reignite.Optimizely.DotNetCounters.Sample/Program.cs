using System.Runtime.InteropServices;
using Reignite.Optimizely.DotNetCounters.Services;

// A runnable host for the counters that needs NO Optimizely licence.
//
// An Optimizely CMS site needs a licence key to start, which makes "try the counters" hard for
// anyone who has not bought one. The counters themselves do not: the package registers an
// Application Insights EventCounter collection module, and EventCounters are a runtime feature.
// AddOptimizelyPerformanceCounters is the non-EPiServer registration path, so a plain ASP.NET
// Core host can run the exact counter set a V12/V13 site runs.
//
// What this cannot show is the CMS-side decorators (content, cache, events) - those need
// EPiServer types and therefore a licensed site. This sample deliberately covers the half that
// is licence-free rather than pretending to cover both.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptimizelyPerformanceCounters(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "reignite-dotnetcounters-sample",
    framework = RuntimeInformation.FrameworkDescription,
    note = "No Optimizely licence required. Counters are published as EventCounters."
}));

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Allocation: drives gen sizes, LOH size and allocation rate.
app.MapGet("/allocate", (int mb) =>
{
    var megabytes = Math.Clamp(mb, 1, 256);
    var block = new byte[megabytes * 1024 * 1024];
    block[^1] = 1;
    return Results.Ok(new { allocatedMb = megabytes });
});

// CPU: drives cpu-usage and thread pool counters.
app.MapGet("/spin", (int ms) =>
{
    var elapsed = Math.Clamp(ms, 1, 5000);
    var deadline = Environment.TickCount64 + elapsed;
    while (Environment.TickCount64 < deadline) { }
    return Results.Ok(new { spunMs = elapsed });
});

// Contention: drives monitor-lock-contention-count.
app.MapGet("/contention", (int workers) =>
{
    var count = Math.Clamp(workers, 2, 32);
    var gate = new object();
    Parallel.For(0, count, _ => { lock (gate) { Thread.Sleep(20); } });
    return Results.Ok(new { workers = count });
});

app.Run();

/// <summary>Exposed so the sample can be hosted by WebApplicationFactory.</summary>
public partial class Program
{
}
