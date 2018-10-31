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
using System.Timers;
using log4net;
using ZF.BL.Nesper.Utils;
using ZF.BL.Nesper.Wcf.Client;

namespace ZF.BL.Nesper.Model
{
    public class NesperPerformance
    {
        private static long _prevEvents, _prevAlerts;
        private readonly ILog _perfLog;
        private HardwareInfoRetriever _hardwareRetriever;
        private PerformanceStatsRetriever _perfStatsRetriever;
        private Timer _perfTimer;
        private readonly PerformanceProxy _proxy;
        private int _saveInterval;
        private Stopwatch _sw;

        public NesperPerformance(ILog perfLog, PerformanceProxy proxy)
        {
            _perfLog = perfLog;
            _proxy = proxy;
        }

        public static long TotalEvents { get; set; }
        public static long TotalAlerts { get; set; }

        public void Start(int saveIntervalInSeconds)
        {
            _saveInterval = saveIntervalInSeconds;

            _perfTimer = new Timer(TimeSpan.FromSeconds(_saveInterval).TotalMilliseconds);
            _perfTimer.Elapsed += OnPerfTimerElapsed;
            _perfTimer.AutoReset = true;
            GC.KeepAlive(_perfTimer);

            _perfStatsRetriever = new PerformanceStatsRetriever();

            _hardwareRetriever = new HardwareInfoRetriever(LogManager.GetLogger(BlLog.ExceptionLog));
            string cpu = _hardwareRetriever.GetCpuNameAndSpeed();
            int nCores = _hardwareRetriever.GetNumberOfCores();
            int totalRam = _hardwareRetriever.GetTotalRamInMb();
            int memBusSpeed = _hardwareRetriever.GetMemoryBusSpeed();
            string platform = _hardwareRetriever.GetPlatformName();
            string ip = _hardwareRetriever.GetIpAddresses();

            var logHelper = new LogHelper();
            logHelper.StartTimerFor("Hardware profile saving");
            _proxy.HardwareInfoUpdate(cpu, nCores, totalRam, memBusSpeed, platform, ip);
            logHelper.StopAndLogTime();

            _sw = new Stopwatch();
            _perfTimer.Start();
            _sw.Start();
        }

        private void OnPerfTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            long deltaEvents = TotalEvents - _prevEvents;
            _prevEvents = TotalEvents;

            long deltaAlerts = TotalAlerts - _prevAlerts;
            _prevAlerts = TotalAlerts;

            double totalSec = _sw.Elapsed.TotalSeconds;

            double eventRate = deltaEvents/totalSec;
            double alertRate = deltaAlerts/totalSec;

            _perfLog.Info(
                $"Total: {TotalEvents:n0} events {TotalAlerts:n0} alerts. Average {eventRate:n1} events/sec and {alertRate:n1} alerts/sec");

            int cpu = _perfStatsRetriever.GetCpuPercentageUsedByCurrentProcess();
            int memFree = _perfStatsRetriever.GetFreeSystemMemory();
            int memUsed = _perfStatsRetriever.GetMemoryUsedByCurrentProcess();
            _proxy.PerformanceInfoUpdate(cpu, memUsed, memFree, (int) eventRate, (int) alertRate, TotalEvents, TotalAlerts);

            _sw.Restart();
        }
    }
}