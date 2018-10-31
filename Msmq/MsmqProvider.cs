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
using System.Messaging;

namespace ZF.BL.Nesper.Msmq
{
    public class MsmqProvider<T>
    {
        private MessageQueue _messageQueue;

        public void SetUp(string queuePath)
        {
            if (string.IsNullOrEmpty(queuePath))
                throw new ArgumentNullException();

            bool msmqExist;
            try
            {
                msmqExist = MessageQueue.Exists(queuePath);
            }
            catch (InvalidOperationException)
            {
                msmqExist = true;
            }

            _messageQueue = !msmqExist
                                ? MessageQueue.Create(queuePath, false)
                                : new MessageQueue(queuePath, false, true, QueueAccessMode.Receive);

            _messageQueue.Formatter = new JsonGzipMessageFormatter<T>();
            var filter = new MessagePropertyFilter();
            filter.ClearAll();
            filter.Body = true;
            _messageQueue.MessageReadPropertyFilter = filter;
        }

        public T Receive(TimeSpan timeout)
        {
            Message msg = _messageQueue.Receive(timeout);
            if (msg == null)
            {
                return default(T);
            }
            return (T) msg.Body;
        }

        public void Dispose()
        {
            if (_messageQueue != null)
            {
                _messageQueue.Close();
                _messageQueue.Dispose();
            }
        }
    }
}