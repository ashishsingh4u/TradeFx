//  ===================================================================================
//  <copyright file="PgmReceiver.cs" company="TechieNotes">
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
//     The PgmReceiver.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Net;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The on receive.</summary>
    /// <param name="endPoint">The end point.</param>
    /// <param name="data">The data.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    public delegate void OnReceive(EndPoint endPoint, byte[] data, int offset, int length);

    /// <summary>The pgm receiver.</summary>
    public class PgmReceiver : IDisposable, IAcceptor
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmReceiver));

        #endregion

        #region Fields

        /// <summary>The _dispose lock.</summary>
        private readonly object _disposeLock = new object();

        /// <summary>The _interface addresses.</summary>
        private readonly IList<uint> _interfaceAddresses = new List<uint>();

        /// <summary>The _reader.</summary>
        private readonly ISourceReader _reader;

        /// <summary>The _socket.</summary>
        private readonly PgmSocket _socket;

        /// <summary>The _ip.</summary>
        private string _ip;

        /// <summary>The _port.</summary>
        private int _port;

        /// <summary>The _receive buffer in bytes.</summary>
        private int _receiveBufferInBytes = 1024 * 128;

        /// <summary>The _running.</summary>
        private bool _running = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PgmReceiver"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="reader">The reader.</param>
        public PgmReceiver(string address, int port, ISourceReader reader)
        {
            _socket = new PgmSocket();
            _ip = address;
            _port = port;
            _reader = reader;
        }

        #endregion

        #region Public Properties

        /// <summary>Sets the address.</summary>
        public string Address
        {
            set
            {
                _ip = value;
            }
        }

        /// <summary>Gets a value indicating whether is running.</summary>
        public bool IsRunning
        {
            get
            {
                return _running;
            }
        }

        /// <summary>Sets the port.</summary>
        public int Port
        {
            set
            {
                _port = value;
            }
        }

        /// <summary>Gets or sets the receive buffer in bytes.</summary>
        public int ReceiveBufferInBytes
        {
            get
            {
                return _receiveBufferInBytes;
            }

            set
            {
                _receiveBufferInBytes = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Set the address for binding the socket</summary>
        /// <param name="address">The address.</param>
        public void AddInterfaceAddress(string address)
        {
            var ip = IPAddress.Parse(address);
            _interfaceAddresses.Add((uint)ip.Address);
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_running)
                {
                    _running = false;
                    try
                    {
                        _socket.Close();
                    }
                    catch (Exception failed)
                    {
                        log.Warn("close failed", failed);
                    }
                }
            }
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            _socket.ReceiveBufferSize = _receiveBufferInBytes;
            var ipAddr = IPAddress.Parse(_ip);
            var end = new IPEndPoint(ipAddr, _port);
            _socket.Bind(end);
            foreach (var address in _interfaceAddresses)
            {
                var bits = BitConverter.GetBytes(address);
                _socket.SetPgmOption(PgmConstants.RM_ADD_RECEIVE_IF, bits);
            }

            _socket.ApplySocketOptions();
            PgmSocket.EnableGigabit(_socket);
            _socket.Listen(5);
            log.Info("Listening: " + end);
            _socket.BeginAccept(OnAccept, null);
        }

        #endregion

        #region Methods

        /// <summary>The on accept.</summary>
        /// <param name="ar">The ar.</param>
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                var conn = _socket.EndAccept(ar);
                log.Info("Connection from: " + conn.RemoteEndPoint);
                _reader.AcceptSocket(conn, this);
                _socket.BeginAccept(OnAccept, null);
            }
            catch (Exception failed)
            {
                if (_running)
                {
                    log.Warn("Accept Failed", failed);
                }
            }
        }

        #endregion
    }
}