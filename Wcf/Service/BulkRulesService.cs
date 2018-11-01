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
using System.ServiceModel;
using log4net;
using ZF.BL.Nesper.Model;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BulkRulesService : IBulkRules
    {
        private readonly IBulkRulesManager _bulkRulesManagerMgr;
        private readonly ILog _log;
        private readonly WcfCall _wcfCall;

        public BulkRulesService(IBulkRulesManager bulkRulesManagerMgr)
        {
            if (bulkRulesManagerMgr == null) throw new ArgumentNullException("bulkRulesManagerMgr");

            _bulkRulesManagerMgr = bulkRulesManagerMgr;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _wcfCall = new WcfCall();
        }

        public bool ReloadRules(Dictionary<string, string> idEplDictionary)
        {
            return _wcfCall.Wrap(() =>
                {
                    _log.Warn("Received request to reload all rules");

                    _bulkRulesManagerMgr.UnloadAll();

                    _bulkRulesManagerMgr.LoadAll(idEplDictionary);

                    return true;
                });
        }
    }
}