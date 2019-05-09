//  ===================================================================================
//  <copyright file="PgmSource.cs" company="TechieNotes">
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
//     The PgmSource.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The pgm source.</summary>
    public class PgmSource : IDisposable, IByteWriter
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmSource));

        #endregion

        #region Fields

        /// <summary>The _ip.</summary>
        private readonly string _ip;

        /// <summary>The _port.</summary>
        private readonly int _port;

        /// <summary>The _socket.</summary>
        private readonly PgmSocket _socket;

        /// <summary>The _bind interface.</summary>
        private string _bindInterface;

        /// <summary>The _bind port.</summary>
        private int _bindPort;

        /// <summary>The _rate kbits per sec.</summary>
        private uint _rateKbitsPerSec = 1024 * 10;

        /// <summary>The _send socket size.</summary>
        private int _sendSocketSize = 1024 * 1024;

        /// <summary>The _window size in bytes.</summary>
        private uint _windowSizeInBytes = 1000 * 1000 * 10;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PgmSource"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public PgmSource(string address, int port)
        {
            _socket = new PgmSocket();
            _ip = address;
            _port = port;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the bind interface.</summary>
        public string BindInterface
        {
            get
            {
                return _bindInterface;
            }

            set
            {
                this._bindInterface = value;
            }
        }

        /// <summary>Gets or sets the bind port.</summary>
        public int BindPort
        {
            get
            {
                return _bindPort;
            }

            set
            {
                this._bindPort = value;
            }
        }

        /// <summary>Gets or sets the rate kbits per sec.</summary>
        public uint RateKbitsPerSec
        {
            get
            {
                return _rateKbitsPerSec;
            }

            set
            {
                this._rateKbitsPerSec = value;
            }
        }

        /// <summary>Gets the socket.</summary>
        public PgmSocket Socket
        {
            get
            {
                return _socket;
            }
        }

        /// <summary>Gets or sets the socket buffer size.</summary>
        public int SocketBufferSize
        {
            get
            {
                return _sendSocketSize;
            }

            set
            {
                this._sendSocketSize = value;
            }
        }

        /// <summary>Gets or sets the window size in m secs.</summary>
        public uint WindowSizeInMSecs { get; set; }

        /// <summary>Gets or sets the window sizein bytes.</summary>
        public uint WindowSizeinBytes
        {
            get
            {
                return _windowSizeInBytes;
            }

            set
            {
                this._windowSizeInBytes = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            try
            {
                _socket.Close();
            }
            catch (Exception failed)
            {
                log.Warn("close failed", failed);
            }
        }

        /// <summary>The get send window.</summary>
        /// <returns>The <see cref="_RM_SEND_WINDOW"/>.</returns>
        public unsafe _RM_SEND_WINDOW GetSendWindow()
        {
            var size = sizeof(_RM_SEND_WINDOW);
            var data = _socket.GetSocketOption(PgmSocket.PGM_LEVEL, (SocketOptionName)1001, size);
            fixed (byte* pBytes = &data[0])
            {
                return *((_RM_SEND_WINDOW*)pBytes);
            }
        }

        /// <summary>The get sender stats.</summary>
        /// <returns>The <see cref="_RM_SENDER_STATS"/>.</returns>
        public unsafe _RM_SENDER_STATS GetSenderStats()
        {
            var size = sizeof(_RM_SENDER_STATS);
            var data = _socket.GetSocketOption(PgmSocket.PGM_LEVEL, (SocketOptionName)1005, size);
            fixed (byte* pBytes = &data[0])
            {
                return *((_RM_SENDER_STATS*)pBytes);
            }
        }

        /// <summary>The publish.</summary>
        /// <param name="dataToPublish">The data to publish.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int Publish(params byte[] dataToPublish)
        {
            return _socket.Send(dataToPublish);
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            var ipAddr = IPAddress.Parse(_ip);
            var end = new IPEndPoint(ipAddr, _port);
            _socket.SendBufferSize = _sendSocketSize;
            var local = IPAddress.Any;
            if (_bindInterface != null)
            {
                local = IPAddress.Parse(_bindInterface);
            }

            _socket.Bind(new IPEndPoint(local, _bindPort));
            SetSendWindow();
            PgmSocket.EnableGigabit(_socket);
            _socket.ApplySocketOptions();
            _socket.Connect(end);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="msWaitIgnored">The ms wait ignored.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Write(byte[] data, int offset, int length, int msWaitIgnored)
        {
            _socket.Send(data, offset, length, SocketFlags.None);
            return true;
        }

        #endregion

        #region Methods

        /// <summary>The set send window.</summary>
        private void SetSendWindow()
        {
            var window = new _RM_SEND_WINDOW();
            window.RateKbitsPerSec = RateKbitsPerSec;
            window.WindowSizeInMSecs = WindowSizeInMSecs;
            window.WindowSizeInBytes = WindowSizeinBytes;
            var allData = PgmSocket.ConvertStructToBytes(window);
            _socket.SetSocketOption(PgmSocket.PGM_LEVEL, (SocketOptionName)1001, allData);
        }

        #endregion
    }
}