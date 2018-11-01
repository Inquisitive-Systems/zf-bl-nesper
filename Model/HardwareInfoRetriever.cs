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
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace ZF.BL.Nesper.Model
{
    public class HardwareInfoRetriever
    {
        private const string CpuName = "Name";
        private const string TotalRam = "TotalVisibleMemorySize";
        private const string MemorySpeed = "Speed";
        private const string PlatformName = "Caption";

        private const string Win32Processor = "Win32_Processor";
        private const string Win32OperatingSystem = "Win32_OperatingSystem";
        private const string Win32PhysicalMemory = "Win32_PhysicalMemory";

        private readonly ILog _exceptionLog;

        public HardwareInfoRetriever(ILog exceptionLog)
        {
            _exceptionLog = exceptionLog;
        }

        public string GetCpuNameAndSpeed()
        {
            string cpuName = "CPU NAME NOT FOUND";

            try
            {
                ManagementObjectCollection getProcessorName = RunQuery(CpuName, Win32Processor);
                if (getProcessorName != null)
                {
                    foreach (ManagementBaseObject obj in getProcessorName)
                    {
                        if (obj == null)
                            continue;

                        string tmp = obj[CpuName].ToString();
                        cpuName = string.IsNullOrWhiteSpace(tmp)
                                      ? "CPU NAME NOT FOUND"
                                      : tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLog.Error("Failed to execute GetCpuNameAndSpeed", ex);
            }

            return cpuName;
        }

        public int GetNumberOfCores()
        {
            return Environment.ProcessorCount;
        }

        public int GetTotalRamInMb()
        {
            int totalRam = -1;

            try
            {
                ManagementObjectCollection getTotalRam = RunQuery(TotalRam, Win32OperatingSystem);
                if (getTotalRam != null)
                {
                    IEnumerable<string> items = from ManagementBaseObject obj in getTotalRam
                                                select obj[TotalRam].ToString();

                    foreach (string item in items)
                    {
                        if (item == null)
                            continue;

                        totalRam = string.IsNullOrWhiteSpace(item) ? -1 : int.Parse(item)/1024;
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLog.Error("Failed to execute GetTotalRamInMb", ex);
            }

            return totalRam;
        }

        public int GetMemoryBusSpeed()
        {
            int memorySpeed = -1;

            try
            {
                ManagementObjectCollection queryResult = RunQuery(
                    MemorySpeed.ToLowerInvariant(),
                    Win32PhysicalMemory);

                if (queryResult != null)
                {
                    IEnumerable<object> values = (from ManagementBaseObject obj in queryResult
                                                  select obj[MemorySpeed]);

                    foreach (object value in values)
                    {
                        if (value == null)
                            continue;

                        // select the fastest ram installed if there are several of them
                        int tmpValue = string.IsNullOrWhiteSpace(value.ToString())
                                           ? -3
                                           : int.Parse(value.ToString());

                        if (tmpValue > memorySpeed)
                            memorySpeed = tmpValue;
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLog.Error("Failed to execute GetMemoryBusSpeed", ex);
            }

            return memorySpeed;
        }

        public string GetPlatformName()
        {
            string platformName = "PLATFORM NAME NOT FOUND";

            try
            {
                ManagementObjectCollection getPlatformName = RunQuery(PlatformName, Win32OperatingSystem);
                if (getPlatformName != null)
                {
                    IEnumerable<string> items = from ManagementBaseObject obj in getPlatformName
                                                select obj[PlatformName].ToString();

                    foreach (string item in items)
                    {
                        if (item == null)
                            continue;

                        platformName = string.IsNullOrWhiteSpace(item)
                                           ? "PLATFORM NAME NOT FOUND"
                                           : item;
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLog.Error("Failed to execute GetPlatformName", ex);
            }


            return platformName;
        }

        public string GetIpAddresses()
        {
            string ips = "";

            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                int counter = 0;
                List<IPAddress> interNetworkIps = host.AddressList
                                                      .Select(x => x)
                                                      .Where(y => y.AddressFamily == AddressFamily.InterNetwork)
                                                      .ToList();

                foreach (IPAddress ipAddress in interNetworkIps)
                {
                    if (ipAddress == null)
                        continue;

                    if (counter == interNetworkIps.Count - 1)
                    {
                        ips += ipAddress;
                    }
                    else
                    {
                        ips += string.Format("{0}, ", ipAddress);
                    }

                    //add one to the counter
                    counter++;
                }

                if (string.IsNullOrWhiteSpace(ips))
                    return "IP ADDRESS NOT FOUND";
            }
            catch (Exception ex)
            {
                _exceptionLog.Error("Failed to execute GetIpAddresses", ex);
            }

            return ips;
        }

        private ManagementObjectCollection RunQuery(string column, string tablename)
        {
            try
            {
                var searcher = new ManagementObjectSearcher(
                    string.Format("SELECT {0} from {1}", column, tablename));
                return searcher.Get();
            }
            catch (Exception ex)
            {
                _exceptionLog.Error(
                    string.Format("Unable to retrieve column {0} from table {1}",
                                  column, tablename), ex);
            }

            return null;
        }
    }
}