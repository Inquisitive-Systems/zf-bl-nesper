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
using System.Diagnostics;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    public class BulkRulesManager : IBulkRulesManager
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly ILog _log;

        public BulkRulesManager(IEventProcessor eventProcessor)
        {
            if (eventProcessor == null) throw new ArgumentNullException(nameof(eventProcessor));

            _eventProcessor = eventProcessor;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
        }

        public void LoadAll(Dictionary<string, string> idEplDictionary, bool exitOnFailure = false)
        {
            var logHelper = new LogHelper();
            logHelper.StartTimerFor("Rules insertion");
            foreach (var item in idEplDictionary)
            {
                try
                {
                    _eventProcessor.Load(item.Key, item.Value);
                    _log.Error($"Loaded rule id {item.Key}");
                }
                catch (ArgumentException)
                {
                    _log.Error($"Failed to load rule id {item.Key}");
                    if (exitOnFailure)
                        throw;
                }
            }
            logHelper.StopAndLogTime();
        }

        public void UnloadAll()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _eventProcessor.UnloadAll();
            sw.Stop();
            _log.Debug($"All unloaded in {sw.ElapsedMilliseconds} ms");
        }
    }
}