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
using System.ServiceModel;
using log4net;
using ZF.BL.Nesper.Model;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RuleService : IRule
    {
        private readonly IEventProcessor _adapter;
        private readonly ILog _log;
        private readonly WcfCall _wcfCall;

        public RuleService(IEventProcessor adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");

            _adapter = adapter;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _wcfCall = new WcfCall();
        }

        public bool Add(string id, string epl)
        {
            _log.Debug("Adding rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    var logHelper = new LogHelper();
                    logHelper.LogRuleDetail(epl, id);

                    _log.Debug(id + " > loading rule to NEsper");
                    _adapter.Load(id, epl);

                    return true;
                });
        }

        public bool Update(string id, string epl)
        {
            _log.Debug("Updating rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    _adapter.Unload(id);
                    return Add(id, epl);
                });
        }

        public bool Remove(string id)
        {
            _log.Debug("Removing rule");
            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    _log.Debug(id + " > unloading rule from NEsper");
                    _adapter.Unload(id);

                    return true;
                });
        }

        public bool Validate(string id, string epl)
        {
            // allow id to be null or empty

            _log.Debug("Validating rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");

                    var logHelper = new LogHelper();
                    logHelper.LogRuleDetail(epl, id);

                    _log.Debug(id + " > Validating rule in NEsper");
                    _adapter.Validate(id, epl);

                    return true;
                });
        }
    }
}