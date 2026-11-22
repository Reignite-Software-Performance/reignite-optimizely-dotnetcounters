# Licence-free example: run the counters without Optimizely CMS

An Optimizely CMS site will not start without a licence key. The counters do not need one, so this
sample runs the **same EventCounter set** a V12/V13 site runs, in a plain ASP.NET Core 8 host.

What it proves: the counter registration, the counter names, and that the numbers move under load.
What it does not prove: the CMS-side decorators (content loads, cache hit rate, cache cascade). Those
need EPiServer types and therefore a licensed site — see the main [README](../../README.md).

## Run it

```bash
dotnet run --project examples/Reignite.Optimizely.DotNetCounters.Sample
```

## Watch the counters

Two ways, both without a licence and without an Azure subscription:

**Live, in one terminal** — the runtime's own tool, reading the same EventSource the package
subscribes to:

```bash
dotnet-counters monitor --name Reignite.Optimizely.DotNetCounters.Sample System.Runtime Microsoft.AspNetCore.Hosting
```

**Under load, in another** — the sample exposes the three workloads the counters are tuned for:

```bash
curl http://localhost:5000/spin?ms=500          # cpu-usage, threadpool-*
curl "http://localhost:5000/allocate?mb=64"     # gc-heap-size, gen sizes, loh-size
curl "http://localhost:5000/contention?workers=8"  # monitor-lock-contention-count
```

If you have an Application Insights resource, set `APPLICATIONINSIGHTS_CONNECTION_STRING` before
running and the same numbers land in `customMetrics` instead of on the console — that is the
production path the package is built for.

## Configuration

The sample sets only `Optimizely:PerformanceCounters:Enabled`. Every other value has a default; the
full set is in [../appsettings.json.example](../appsettings.json.example).
