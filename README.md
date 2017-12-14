# Nexogen.Libraries.Metrics

Library for collecting application metrics in .Net and exporting them to [Prometheus](https://prometheus.io/)

[![Build Status](https://travis-ci.org/nexogen-international/Nexogen.Libraries.Metrics.svg?branch=master)](https://travis-ci.org/nexogen-international/Nexogen.Libraries.Metrics)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/nexogen-international/Nexogen.Libraries.Metrics/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Nexogen.Libraries.Metrics.svg)](https://www.nuget.org/packages/Nexogen.Libraries.Metrics.Prometheus/)
[![NuGet](https://img.shields.io/nuget/dt/Nexogen.Libraries.Metrics.svg)](https://github.com/nexogen-international/Nexogen.Libraries.Metrics)
[![GitHub issues](https://img.shields.io/github/issues/nexogen-international/Nexogen.Libraries.Metrics.svg)](https://github.com/nexogen-international/Nexogen.Libraries.Metrics/issues)
[![GitHub stars](https://img.shields.io/github/stars/nexogen-international/Nexogen.Libraries.Metrics.svg)](https://github.com/nexogen-international/Nexogen.Libraries.Metrics/stargazers)
[![Twitter](https://img.shields.io/twitter/url/https/github.com/nexogen-international/Nexogen.Libraries.Metrics.svg?style=social)](https://twitter.com/intent/tweet?text=Wow:&url=%5Bobject%20Object%5D)

# Installation

```sh
dotnet add package Nexogen.Libraries.Metrics.Prometheus
dotnet add package Nexogen.Libraries.Metrics.Extensions
```

You can use an interface only nuget when writing libraries or when you want to use Metrics through dependency injection.

```sh
dotnet add package Nexogen.Libraries.Metrics
```

For exporting metrics you currently have to use ASP.NET Core.

```sh
dotnet add package Nexogen.Libraries.Metrics.Prometheus.AspCore
```

Or you can use a push gateway when measuring batch processes.

```sh
dotnet add package Nexogen.Libraries.Metrics.Prometheus.PushGateway
```

# Example usage

## Counters

Counters can only increment, so they are most useful for counting things, like calls to API endpoints or backend services.

```cs
            IMetrics metrics = new PrometheusMetrics();

            ICounter counter = metrics.Counter()
                .Name("nexogen_sort_calls_total")
                .Help("Total calls to sort routine.")
                .Register();

            counter.Increment();
```

## Gauges

Gauges can take any value, so they are the most versatile metric type available. You can even measure durations or dates with them!

```cs
            IGauge gauge = metrics.Gauge()
                .Name("nexogen_sorted_items_count_last")
                .Help("The last count of the sorted items.")
                .Register();
                
            gauge.Value = items.Length;

            gauge.Increment();
            gauge.Decrement(10.1);           
```
## Histograms

Histograms are a trade off between measuring resolution and precision. With histograms you can avoid aliasing errors from Prometheus's scrape interval, but lose granularity. Histograms also need to have their buckets defined before use and we provide sevaral bucket generators to make it easy.

```cs

            IHistogram histogram = metrics.Histogram()
                .LinearBuckets(0.01, 0.01, 100)
                .Name("nexogen_sort_time_seconds")
                .Help("Time taken for sort in seconds.")
                .Register();

            var sw = Stopwatch.StartNew();
            Array.Sort(items);
            histogram.Observe(sw.Elapsed.TotalSeconds);

```

## Extensions

We provide an [Extensions](https://www.nuget.org/packages/Nexogen.Libraries.Metrics.Extensions) library for making common measurements easy.

```cs
            using (histogram.Timer())
            {
                Array.Sort(items);
            }

            gauge.SetToCurrentTime();

            gauge.TrackInProgress(() => Array.Sort(items));
```
## Standalone server

There is a standalone server if you don't want to use ASP.NET Core just to expose your metrics.

```sh
dotnet add package Nexogen.Libraries.Metrics.Prometheus.Standalone
```

```cs
            var metrics = new PrometheusMetrics();                                                                                                                                                                                                                                  
            metrics.Server().Port(9100).Start();
```
