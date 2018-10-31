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
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    public class WcfCall
    {
        private readonly ILog _exLog;

        public WcfCall()
        {
            _exLog = LogManager.GetLogger(BlLog.ExceptionLog);
        }

        public void Wrap(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public TResult Wrap<TResult>(Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                return action.Invoke();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        private FaultException HandleException(Exception ex)
        {
            //we need to look at the exception inside here and determine what on earth to do with it.
            _exLog.Error(ex.ToString());
            ZoneFoxFault fault = ZoneFoxFault.CreateFrom(ex);

            string reason = !string.IsNullOrWhiteSpace(fault.InnerException) 
                ? string.Format("{0}", fault.InnerException) 
                : string.Format("{0}", fault.Message);

            return new FaultException<ZoneFoxFault>(fault, new FaultReason(reason));
        }
    }
}