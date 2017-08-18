using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public class CoreclrExporter
    {
        private ILabelledGauge GcCollectionCount;

        private IGauge NonpagedSystemMemorySize;
        private IGauge PagedMemorySize;
        private IGauge PagedSystemMemorySize;
        private IGauge PeakPagedMemorySize;
        private IGauge PeakVirtualMemorySize;
        private IGauge PeakWorkingSet;
        private IGauge PrivateMemorySize;
        private IGauge VirtualMemorySize;
        private IGauge WorkingSet;

        private IGauge PrivilegedProcessorTime;
        private IGauge UserProcessorTime;
        private IGauge TotalProcessorTime;
        private IGauge ThreadCount;

        private IGauge ProcessCpuSecondsTotal;
        private IGauge ProcessOpenFds;
        private IGauge ProcessMaxFds;
        private IGauge ProcessVirtualMemoryBytes;
        private IGauge ProcessResidentMemoryBytes;
        private IGauge ProcessHeapBytes;
        private IGauge ProcessStartTimeSeconds;

        private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

        private readonly Lazy<object> init;

        public CoreclrExporter(IMetrics metrics)
        {
            this.init = new Lazy<object>(() =>
            {
                Register(metrics);
                return null;
            });
        }

        private void Register(IMetrics metrics)
        {
            GcCollectionCount = metrics.Gauge()
                .Name("coreclr_gc_collection_count")
                .Help("The number of times a collection has occured in the given generation")
                .LabelNames("generation")
                .Register();

            NonpagedSystemMemorySize = metrics.Gauge()
                .Name("coreclr_process_nonpaged_system_memory_size_bytes")
                .Help("Pool Nonpaged Bytes")
                .Register();

            PagedMemorySize = metrics.Gauge()
                .Name("coreclr_process_paged_memory_size_bytes")
                .Help("Page File Bytes")
                .Register();

            PagedSystemMemorySize = metrics.Gauge()
                .Name("coreclr_process_paged_system_memory_size_bytes")
                .Help("Pool Paged Bytes")
                .Register();

            PeakPagedMemorySize = metrics.Gauge()
                .Name("coreclr_process_peak_paged_memory_size_bytes")
                .Help("Page File Bytes Peak")
                .Register();

            PeakVirtualMemorySize = metrics.Gauge()
                .Name("coreclr_process_peak_virtual_memory_size_bytes")
                .Help("Virtual Bytes Peak")
                .Register();

            PeakWorkingSet = metrics.Gauge()
                .Name("coreclr_process_peak_working_set_bytes")
                .Help("Working Set Peak")
                .Register();

            PrivateMemorySize = metrics.Gauge()
                .Name("coreclr_process_private_memory_size_bytes")
                .Help("Private Bytes")
                .Register();

            VirtualMemorySize = metrics.Gauge()
                .Name("coreclr_process_virtual_size_bytes")
                .Help("Virtual Bytes")
                .Register();

            WorkingSet = metrics.Gauge()
                .Name("coreclr_working_set_bytes")
                .Help("Working Set")
                .Register();

            ThreadCount = metrics.Gauge()
                .Name("coreclr_process_thread_count")
                .Help("The number of threads in the .NET process")
                .Register();

            UserProcessorTime = metrics.Gauge()
                .Name("coreclr_process_user_processor_time_seconds")
                .Help("The user processor time for this process")
                .Register();

            PrivilegedProcessorTime = metrics.Gauge()
                .Name("coreclr_process_privileged_processor_time_seconds")
                .Help("The privileged processor time for this process")
                .Register();

            TotalProcessorTime = metrics.Gauge()
                .Name("coreclr_process_total_processor_time_seconds")
                .Help("The total processor time for this process")
                .Register();

            ProcessCpuSecondsTotal = metrics.Gauge()
                .Name("process_cpu_seconds_total")
                .Help("Total user and system CPU time spent in seconds")
                .Register();

            ProcessOpenFds = metrics.Gauge()
                .Name("process_open_fds")
                .Help("Number of open file descriptors")
                .Register();

            ProcessMaxFds = metrics.Gauge()
                .Name("process_max_fds")
                .Help("Number of open file descriptors")
                .Register();

            ProcessVirtualMemoryBytes = metrics.Gauge()
                .Name("process_virtual_memory_bytes")
                .Help("Virtual memory size in bytes")
                .Register();

            ProcessResidentMemoryBytes = metrics.Gauge()
                .Name("process_resident_memory_bytes")
                .Help("Resident memory size in bytes")
                .Register();

            ProcessHeapBytes = metrics.Gauge()
                .Name("process_resident_heap_bytes")
                .Help("Process heap size in bytes")
                .Register();

            ProcessStartTimeSeconds = metrics.Gauge()
                .Name("process_start_time_seconds")
                .Help("Start time of the process since unix epoch in seconds")
                .Register();
        }

        public void Collect()
        {
            //  hack
            var v = init.Value;

            // runtime collectors
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                GcCollectionCount.Labels($"gen{i}").Value = GC.CollectionCount(i);
            }

            var proc = Process.GetCurrentProcess();

            NonpagedSystemMemorySize.Value = proc.NonpagedSystemMemorySize64;
            PagedMemorySize.Value = proc.PagedMemorySize64;
            PagedSystemMemorySize.Value = proc.PagedSystemMemorySize64;
            PeakPagedMemorySize.Value = proc.PeakPagedMemorySize64;
            PeakVirtualMemorySize.Value = proc.PeakVirtualMemorySize64;
            PeakWorkingSet.Value = proc.PeakWorkingSet64;
            PrivateMemorySize.Value = proc.PrivateMemorySize64;
            VirtualMemorySize.Value = proc.VirtualMemorySize64;
            WorkingSet.Value = proc.WorkingSet64;

            PrivilegedProcessorTime.Value = proc.PrivilegedProcessorTime.TotalMilliseconds / 1000.0;
            UserProcessorTime.Value = proc.UserProcessorTime.TotalMilliseconds / 1000.0;
            TotalProcessorTime.Value = proc.TotalProcessorTime.TotalMilliseconds / 1000.0;

            ThreadCount.Value = proc.Threads.Count;

            // standard collectors
            ProcessCpuSecondsTotal.Value = proc.TotalProcessorTime.TotalMilliseconds / 1000.0;

            // coreclr keeps no track of this
            ProcessOpenFds.Value = Double.NaN;
            ProcessMaxFds.Value = Double.NaN;

            ProcessVirtualMemoryBytes.Value = proc.VirtualMemorySize64;
            ProcessResidentMemoryBytes.Value = proc.WorkingSet64;

            // not sure if this is the correct metric for this
            ProcessHeapBytes.Value = proc.PrivateMemorySize64;

            ProcessStartTimeSeconds.Value = Math.Truncate((proc.StartTime - epoch).TotalSeconds);
        }
    }
}
