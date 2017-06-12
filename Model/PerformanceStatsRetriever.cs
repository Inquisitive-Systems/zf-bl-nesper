using System;
using System.Diagnostics;

namespace ZF.BL.Nesper.Model
{
    public class PerformanceStatsRetriever
    {
        private readonly PerformanceCounter _cpucounter = new PerformanceCounter("Process", "% Processor Time",
                                                                                 Process.GetCurrentProcess().ProcessName);

        private readonly PerformanceCounter _freememory = new PerformanceCounter("Memory", "Available MBytes");

        private readonly PerformanceCounter _ramCounter = new PerformanceCounter("Process", "Working Set",
                                                                                 Process.GetCurrentProcess().ProcessName);

        public int GetCpuPercentageUsedByCurrentProcess()
        {
            return Convert.ToInt32(_cpucounter.NextValue());
        }

        public int GetMemoryUsedByCurrentProcess()
        {
            return Convert.ToInt32(_ramCounter.NextValue());
        }

        public int GetFreeSystemMemory()
        {
            return Convert.ToInt32(_freememory.NextValue());
        }
    }
}