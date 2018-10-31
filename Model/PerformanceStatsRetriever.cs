/*
ZoneFox Business Layer event processor based on GNU NEsper
Copyright (C) 2018 ZoneFox

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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