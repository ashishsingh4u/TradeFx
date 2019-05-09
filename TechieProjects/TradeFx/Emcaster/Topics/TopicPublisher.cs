//  ===================================================================================
//  <copyright file="TopicPublisher.cs" company="TechieNotes">
//  ===================================================================================
//   TechieNotes Utilities & Best Practices
//   Samples and Guidelines for Winform & ASP.net development
//  ===================================================================================
//   Copyright (c) TechieNotes.  All rights reserved.
//   THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//   OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//   LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//   FITNESS FOR A PARTICULAR PURPOSE.
//  ===================================================================================
//   The example companies, organizations, products, domain names,
//   e-mail addresses, logos, people, places, and events depicted
//   herein are fictitious.  No association with any real company,
//   organization, product, domain name, email address, logo, person,
//   places, or events is intended or should be inferred.
//  ===================================================================================
//  </copyright>
//  <author>ASHISHSINGH</author>
//  <email>mailto:ashishsingh4u@gmail.com</email>
//  <date>26-01-2013</date>
//  <summary>
//     The TopicPublisher.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Emcaster.Sockets;

namespace Emcaster.Topics
{
    /// <summary>The topic publisher.</summary>
    public class TopicPublisher
    {
        #region Static Fields

        /// <summary>The heade r_ size.</summary>
        public static readonly int HEADER_SIZE = CalculateHeaderSize();

        #endregion

        #region Fields

        /// <summary>The _encoder.</summary>
        private readonly UTF8Encoding _encoder = new UTF8Encoding();

        /// <summary>The _writer.</summary>
        private readonly IByteWriter _writer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TopicPublisher"/> class.</summary>
        /// <param name="writer">The writer.</param>
        public TopicPublisher(IByteWriter writer)
        {
            _writer = writer;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The calculate header size.</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public static unsafe int CalculateHeaderSize()
        {
            return sizeof(MessageHeader);
        }

        /// <summary>The create message.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="encoder">The encoder.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] CreateMessage(string topic, byte[] data, int offset, int length, UTF8Encoding encoder)
        {
            var topicBytes = encoder.GetBytes(topic);
            var header = new MessageHeader(topicBytes.Length, length);
            var totalSize = HEADER_SIZE + header.TotalSize;
            var allData = new byte[totalSize];
            header.WriteToBuffer(allData);
            Array.Copy(topicBytes, 0, allData, HEADER_SIZE, topicBytes.Length);
            Array.Copy(data, offset, allData, HEADER_SIZE + topicBytes.Length, length);
            return allData;
        }

        /// <summary>The publish.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="msToWaitForWriteLock">The ms to wait for write lock.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Publish(string topic, byte[] data, int offset, int length, int msToWaitForWriteLock)
        {
            var allData = CreateMessage(topic, data, offset, length, _encoder);
            return _writer.Write(allData, 0, allData.Length, msToWaitForWriteLock);
        }

        /// <summary>The publish object.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="data">The data.</param>
        /// <param name="msToWaitForWriteLock">The ms to wait for write lock.</param>
        public void PublishObject(string topic, object data, int msToWaitForWriteLock)
        {
            var allData = ToBytes(data);
            Publish(topic, allData, 0, allData.Length, msToWaitForWriteLock);
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            _writer.Start();
        }

        #endregion

        #region Methods

        /// <summary>The to bytes.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        private static byte[] ToBytes(object obj)
        {
            if (obj == null)
            {
                return new byte[0];
            }

            var formatter = new BinaryFormatter();
            var outputStream = new MemoryStream();
            formatter.Serialize(outputStream, obj);
            return outputStream.ToArray();
        }

        #endregion
    }
}