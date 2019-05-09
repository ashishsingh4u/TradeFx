//  ===================================================================================
//  <copyright file="ByteMessageParser.cs" company="TechieNotes">
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
//     The ByteMessageParser.cs file.
//  </summary>
//  ===================================================================================

using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace Emcaster.Topics
{
    /// <summary>The byte message parser.</summary>
    public class ByteMessageParser : IMessageParser
    {
        #region Fields

        /// <summary>The _body.</summary>
        private readonly byte[] _body;

        /// <summary>The _endpoint.</summary>
        private readonly EndPoint _endpoint;

        /// <summary>The _topic.</summary>
        private readonly string _topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ByteMessageParser"/> class.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="body">The body.</param>
        /// <param name="endpoint">The endpoint.</param>
        public ByteMessageParser(string topic, byte[] body, EndPoint endpoint)
        {
            _topic = topic;
            _body = body;
            _endpoint = endpoint;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the end point.</summary>
        public EndPoint EndPoint
        {
            get
            {
                return _endpoint;
            }
        }

        /// <summary>Gets the topic.</summary>
        public string Topic
        {
            get
            {
                return _topic;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The parse bytes.</summary>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public byte[] ParseBytes()
        {
            return _body;
        }

        /// <summary>The parse object.</summary>
        /// <returns>The <see cref="object"/>.</returns>
        public object ParseObject()
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(ParseBytes());
            return formatter.Deserialize(stream);
        }

        #endregion
    }
}