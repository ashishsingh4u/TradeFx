//  ===================================================================================
//  <copyright file="UdpSource.cs" company="TechieNotes">
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
//     The UdpSource.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    /// <summary>The udp source.</summary>
    public class UdpSource : IByteWriter, IDisposable
    {
        #region Fields

        /// <summary>The _client.</summary>
        private readonly UdpClient _client;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UdpSource"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public UdpSource(string address, int port)
        {
            _client = new UdpClient(address, port);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the client.</summary>
        public UdpClient Client
        {
            get
            {
                return _client;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _client.Close();
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            // nothing to do
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="msWaitIgnored">The ms wait ignored.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Write(byte[] data, int offset, int length, int msWaitIgnored)
        {
            if (offset == 0)
            {
                _client.Send(data, length);
            }
            else
            {
                var bytes = new byte[length];
                Array.Copy(data, offset, bytes, 0, length);
                _client.Send(bytes, length);
            }

            return true;
        }

        #endregion
    }
}