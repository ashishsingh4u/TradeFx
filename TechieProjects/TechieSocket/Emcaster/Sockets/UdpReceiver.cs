//  ===================================================================================
//  <copyright file="UdpReceiver.cs" company="TechieNotes">
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
//     The UdpReceiver.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The udp receiver.</summary>
    public class UdpReceiver : IPacketEvent, IDisposable
    {
        #region Static Fields

        /// <summary>The log.</summary>
        public static readonly ILog log = LogManager.GetLogger(typeof(UdpReceiver));

        #endregion

        #region Fields

        /// <summary>The _address.</summary>
        private readonly IPAddress _address;

        /// <summary>The _client.</summary>
        private readonly UdpClient _client;

        /// <summary>The _runner.</summary>
        private readonly AsyncCallback _runner;

        /// <summary>The _running.</summary>
        private bool _running = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UdpReceiver"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public UdpReceiver(string address, int port)
        {
            _client = new UdpClient(port);
            _address = IPAddress.Parse(address);
            _runner = delegate(IAsyncResult ar)
                {
                    try
                    {
                        Receive(ar);
                    }
                    catch (Exception failed)
                    {
                        log.Warn("read failed. ending connection: " + _address, failed);
                    }
                };
        }

        #endregion

        #region Public Events

        /// <summary>The receive event.</summary>
        public event OnReceive ReceiveEvent;

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
            _running = false;
            _client.Close();
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            _client.JoinMulticastGroup(_address);
            _client.BeginReceive(_runner, null);
        }

        #endregion

        #region Methods

        /// <summary>The receive.</summary>
        /// <param name="result">The result.</param>
        private void Receive(IAsyncResult result)
        {
            IPEndPoint endpoint = null;
            var packet = _client.EndReceive(result, ref endpoint);
            var rcv = ReceiveEvent;
            if (rcv != null)
            {
                rcv(endpoint, packet, 0, packet.Length);
            }

            _client.BeginReceive(_runner, null);
        }

        #endregion
    }
}