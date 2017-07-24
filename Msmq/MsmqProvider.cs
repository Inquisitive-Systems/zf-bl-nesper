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

            _messageQueue.SetPermissions("ANONYMOUS LOGON", MessageQueueAccessRights.FullControl);

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