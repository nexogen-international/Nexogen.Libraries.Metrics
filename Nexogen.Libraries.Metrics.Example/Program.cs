using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Nexogen.Libraries.Metrics.Extensions;
using Nexogen.Libraries.Metrics.Prometheus;
using Nexogen.Libraries.Metrics.Prometheus.PushGateway;

namespace Nexogen.Libraries.Metrics.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IMetrics metrics = new PrometheusMetrics();

            var planningTime = metrics.Gauge()
                .Name("planning_time").Help("The processing time in seconds").LabelNames("solver").Register();

            var planningCount = metrics.Counter()
                .Name("planning_count").Help("The count of planning").LabelNames("solver").Register();

            var totalPlanningCount = metrics.Counter()
                .Name("plannning_count_total").Help("Total count of plannings").Register();

            var totalRunning = metrics.Gauge()
                .Name("plannning_running_total").Help("Currently running plannings").Register();

            var planningStartTime = metrics.Gauge()
                .Name("planning_start_time").Help("Start time of last planning").Register();

            var planningTimeHistogram = metrics.Histogram()
                .LinearBuckets(0, 300, 10)
                .Name("planning_duration_times")
                .Help("Duration of plannings in ms")
                .Register();

            var planningTimeHistogramForSolver = metrics.Histogram()
                .LinearBuckets(0, 300, 10)
                .Name("planning_duration_times_for_solver")
                .Help("Duration of plannings in ms for different solvers")
                .LabelNames("solver")
                .Register();

            var pgw = new PushGateway(new Uri("http://127.0.0.1:9091"));

            Task.Run(async () =>
            {
                var pm = metrics as PrometheusMetrics;
                var output = Console.OpenStandardOutput();

                for (;;)
                {
                    Console.WriteLine("# Waiting");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    Console.WriteLine("# Exposing");
                    await pm.Expose(output);
                    Console.WriteLine("# Done");

                    try
                    {
                        await pgw.PushAsync(pm, "example_app:9000");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            foreach (var solver in new[] { "Static", "Dynamic" })
            {
                Task.Run(() =>
                {
                    var rnd = new Random(solver.GetHashCode());

                    for (;;)
                    {
                        using (planningTime.Labels(solver).Timer())
                        {
                            totalPlanningCount.Increment();
                            planningCount.Labels(solver).Increment();

                            planningStartTime.SetToCurrentTime();

                            totalRunning.TrackInProgress(() =>
                            {
                                var planningSleep = rnd.Next(5000);
                                Thread.Sleep(planningSleep);
                                planningTimeHistogram.Observe(planningSleep);
                                planningTimeHistogramForSolver.Labels(solver).Observe(planningSleep);
                            });
                        }
                    }
                });
            }

            Console.ReadLine();
        }
    }
}
