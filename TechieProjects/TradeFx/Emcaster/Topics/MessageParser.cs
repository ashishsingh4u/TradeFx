//  ===================================================================================
//  <copyright file="MessageParser.cs" company="TechieNotes">
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
//     The MessageParser.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Emcaster.Sockets;

namespace Emcaster.Topics
{
    /// <summary>The message parser.</summary>
    public unsafe class MessageParser : IMessageParser, IByteParser
    {
        #region Fields

        /// <summary>The _decoder.</summary>
        private readonly UTF8Encoding _decoder = new UTF8Encoding();

        /// <summary>The _listener.</summary>
        private readonly IMessageListener _listener;

        /// <summary>The _buffer.</summary>
        private byte[] _buffer;

        /// <summary>The _current header.</summary>
        private MessageHeader* _currentHeader;

        /// <summary>The _end point.</summary>
        private EndPoint _endPoint;

        /// <summary>The _object.</summary>
        private object _object;

        /// <summary>The _offset.</summary>
        private int _offset;

        /// <summary>The _topic.</summary>
        private string _topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MessageParser"/> class.</summary>
        /// <param name="listener">The listener.</param>
        public MessageParser(IMessageListener listener)
        {
            _listener = listener;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the end point.</summary>
        public EndPoint EndPoint
        {
            get
            {
                return _endPoint;
            }
        }

        /// <summary>Gets the topic.</summary>
        public string Topic
        {
            get
            {
                if (_topic == null)
                {
                    var topicSize = _currentHeader->TopicSize;
                    _topic = _decoder.GetString(_buffer, _offset + TopicPublisher.HEADER_SIZE, topicSize);
                }

                return _topic;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The on bytes.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public void OnBytes(EndPoint endpoint, byte[] data, int offset, int length)
        {
            ParseBytes(endpoint, data, offset, length);
        }

        /// <summary>The parse bytes.</summary>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public byte[] ParseBytes()
        {
            var size = _currentHeader->BodySize;
            if (size == 0)
            {
                return new byte[0];
            }

            var result = new byte[size];
            var topicSize = _currentHeader->TopicSize;
            var totalOffset = _offset + topicSize + TopicPublisher.HEADER_SIZE;
            Array.Copy(_buffer, totalOffset, result, 0, size);
            return result;
        }

        /// <summary>The parse bytes.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="received">The received.</param>
        public void ParseBytes(EndPoint endpoint, byte[] buffer, int offset, int received)
        {
            _endPoint = EndPoint;
            _buffer = buffer;
            _offset = offset;
            fixed (byte* pArray = buffer)
            {
                while (_offset < received)
                {
                    _topic = null;
                    _object = null;
                    var pHeader = pArray + _offset;
                    _currentHeader = (MessageHeader*)pHeader;
                    var msgSize = TopicPublisher.HEADER_SIZE + _currentHeader->TopicSize + _currentHeader->BodySize;
                    _listener.OnMessage(this);
                    _offset += msgSize;
                }
            }
        }

        /// <summary>The parse object.</summary>
        /// <returns>The <see cref="object"/>.</returns>
        public object ParseObject()
        {
            if (_object == null)
            {
                var bodySize = _currentHeader->BodySize;
                if (bodySize == 0)
                {
                    return null;
                }

                var formatter = new BinaryFormatter();
                var topicSize = _currentHeader->TopicSize;
                var totalOffset = _offset + topicSize + TopicPublisher.HEADER_SIZE;
                var stream = new MemoryStream(_buffer, totalOffset, bodySize);
                _object = formatter.Deserialize(stream);
            }

            return _object;
        }

        #endregion
    }
}