//  ===================================================================================
//  <copyright file="ByteParser.cs" company="TechieNotes">
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
//     The ByteParser.cs file.
//  </summary>
//  ===================================================================================

using System.Net;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    /// <summary>The byte parser.</summary>
    public class ByteParser : IByteParserFactory, IByteParser, IPacketEvent
    {
        #region Public Events

        /// <summary>The receive event.</summary>
        public event OnReceive ReceiveEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns>The <see cref="IByteParser"/>.</returns>
        public IByteParser Create(Socket socket)
        {
            return this;
        }

        /// <summary>The on bytes.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public void OnBytes(EndPoint endpoint, byte[] data, int offset, int length)
        {
            var onMsg = ReceiveEvent;
            if (onMsg != null)
            {
                onMsg(endpoint, data, offset, length);
            }
        }

        #endregion
    }
}