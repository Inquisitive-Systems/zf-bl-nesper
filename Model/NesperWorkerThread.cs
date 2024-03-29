﻿/*
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
using System.Linq;
using System.Messaging;
using System.Threading;
using log4net;
using ZF.BL.Nesper.Msmq;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    public class NesperWorkerThread
    {
        private readonly IEventProcessor _adapter;
        private readonly ILog _log;
        private readonly string _msmqName;
        private readonly ILog _exLog = LogManager.GetLogger(BlLog.ExceptionLog);

        private volatile bool _isCanceled;
        private MsmqProvider<EventDto[]> _msmqProvider;
        private Thread _workerThread;

        public NesperWorkerThread(IEventProcessor adapter, string msmqName)
        {
            if (msmqName == null) throw new ArgumentNullException("msmqName");
            if (adapter == null) throw new ArgumentNullException("adapter");

            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _msmqName = msmqName;
            _adapter = adapter;
            _isCanceled = false;
        }

        public void StartThread()
        {
            _msmqProvider = new MsmqProvider<EventDto[]>();
            _msmqProvider.SetUp(_msmqName);

            _workerThread = new Thread(WorkToDo)
            {
                Priority = ThreadPriority.AboveNormal,
                Name = "NEsper thread"
            };
            _workerThread.Start();
        }

        private void WorkToDo()
        {
            // while no cancel request made
            while (!_isCanceled)
            {
                // do work
                try
                {
                    EventDto[] events = _msmqProvider.Receive(TimeSpan.FromSeconds(1));
                    //must change them into Activity Events
                    _adapter.PropagateEvents(events.Select(x => x.ToActivityEvent()).ToArray());
                }
                catch (MessageQueueException exception)
                {
                    switch (exception.MessageQueueErrorCode)
                    {
                        // All OK, just timed out, 
                        // we need this to see if cancel is set to true
                        // If we don't timeout we can keep reading from queue
                        // which can locks if nothing goes in
                        case MessageQueueErrorCode.IOTimeout:
                            break;

                        // Something bad happened with msmq, throw exception
                        default:
                            throw;
                    }
                }
                catch (Exception ex)
                {
                    _exLog.Error(ex);
                }
            }

            _log.Debug("MSMQ producer thread complied with cancel request");
        }

        public void StopThread()
        {
            _log.Debug("NEsper thread cancellation requested");
            _isCanceled = true;
            if (_workerThread != null)
                _workerThread.Join();
            if (_msmqProvider != null)
                _msmqProvider.Dispose();
            _log.Debug("Thread stopped");
        }
    }
}