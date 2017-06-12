﻿using System.IO;
using System.IO.Compression;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace ZF.BL.Nesper.Msmq
{
    /// <summary>
    ///     This is a custom MSMQ formatter
    ///     Uses JSON to serialize type and GZip to compress
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the message,
    ///     this can be ActivityEvent[]
    /// </typeparam>
    public class JsonGzipMessageFormatter<T> : IMessageFormatter
    {
        /// <summary>
        ///     This method is used by the msmq.Receive method
        ///     It decompresses message body stream from GZip,
        ///     and deserializes JSON into the type
        /// </summary>
        /// <param name="message">MSMQ message</param>
        /// <returns>Formatted type</returns>
        public object Read(Message message)
        {
            // decompression stream
            using (var stream = new GZipStream(message.BodyStream, CompressionMode.Decompress))
            {
                //temp buffer to hold on decompressed bytes
                var buffer = new byte[16*1024];
                // decompressed stream of bytes
                using (var memory = new MemoryStream())
                {
                    int readCount;
                    while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memory.Write(buffer, 0, readCount);
                    }

                    // at this point we decompressed bytes
                    string json = Encoding.Unicode.GetString(memory.ToArray());
                    // use Newtonsoft JSON to deserialize 
                    var item = JsonConvert.DeserializeObject<T>(json);
                    return item;
                }
            }
        }

        /// <summary>
        ///     This method is used by the msmq.Send method
        ///     It serializes type into JSON and then
        ///     compresses it using GZip
        /// </summary>
        /// <param name="message">MSMQ message</param>
        /// <param name="obj">Data object to send</param>
        public void Write(Message message, object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.None);
            byte[] bytesToCompress = Encoding.Unicode.GetBytes(json);

            using (var outStream = new MemoryStream())
            {
                using (var compressionStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    compressionStream.Write(bytesToCompress, 0, bytesToCompress.Length);
                }

                // copy to new memory stream otherwise the outStream gets disposed
                message.BodyStream = new MemoryStream(outStream.ToArray());
            }
        }

        public object Clone()
        {
            return new JsonGzipMessageFormatter<T>();
        }

        public bool CanRead(Message message)
        {
            return (message.BodyStream != null);
        }
    }
}
