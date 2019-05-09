//  ===================================================================================
//  <copyright file="PgmReader.cs" company="TechieNotes">
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
//     The PgmReader.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net.Sockets;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The Acceptor interface.</summary>
    public interface IAcceptor
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether is running.</summary>
        bool IsRunning { get; }

        #endregion
    }

    /// <summary>The SocketErrorHandler interface.</summary>
    public interface ISocketErrorHandler
    {
        #region Public Methods and Operators

        /// <summary>The on exception.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="failed">The failed.</param>
        void OnException(Socket endpoint, Exception failed);

        /// <summary>The on socket exception.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="failed">The failed.</param>
        void OnSocketException(Socket endpoint, SocketException failed);

        #endregion
    }

    /// <summary>The pgm reader.</summary>
    public class PgmReader : ISourceReader, IPacketEvent, ISocketErrorHandler
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmReader));

        #endregion

        #region Fields

        /// <summary>The _parser factory.</summary>
        private readonly IByteParserFactory _parserFactory;

        /// <summary>The _read buffer.</summary>
        private int _readBuffer = 1024 * 130;

        /// <summary>The _receive buffer size.</summary>
        private int _receiveBufferSize = 1024 * 1024;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PgmReader"/> class.</summary>
        /// <param name="factory">The factory.</param>
        public PgmReader(IByteParserFactory factory)
        {
            _parserFactory = factory;
        }

        #endregion

        #region Public Events

        /// <summary>The exception event.</summary>
        public event OnException ExceptionEvent = delegate { };

        /// <summary>The receive event.</summary>
        public event OnReceive ReceiveEvent;

        /// <summary>The socket exception event.</summary>
        public event OnSocketException SocketExceptionEvent = delegate { };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Set to true to compensate for strange bug in socket protocol.
        ///     Not always needed.
        /// </summary>
        public bool ForceBlockingOnEveryReceive { get; set; }

        /// <summary>Gets or sets the read buffer in bytes.</summary>
        public int ReadBufferInBytes
        {
            get
            {
                return _readBuffer;
            }

            set
            {
                this._readBuffer = value;
            }
        }

        /// <summary>Gets or sets the receive buffer in bytes.</summary>
        public int ReceiveBufferInBytes
        {
            get
            {
                return _receiveBufferSize;
            }

            set
            {
                this._receiveBufferSize = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The accept socket.</summary>
        /// <param name="receiveSocket">The receive socket.</param>
        /// <param name="acceptor">The acceptor.</param>
        public void AcceptSocket(Socket receiveSocket, IAcceptor acceptor)
        {
            try
            {
                var parser = _parserFactory.Create(receiveSocket);
                PgmSocket.EnableGigabit(receiveSocket);
                if (_receiveBufferSize > 0)
                {
                    receiveSocket.ReceiveBufferSize = _receiveBufferSize;
                }

                var buffer = new byte[_readBuffer];
                var reader = new AsyncReader(parser, buffer, acceptor, this, receiveSocket);
                reader.BeginReceive();
            }
            catch (Exception failed)
            {
                receiveSocket.Close();
                log.Error("BeginReceive Failed", failed);
            }
        }

        /// <summary>The on exception.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="error">The error.</param>
        public void OnException(Socket socket, Exception error)
        {
            ExceptionEvent(socket, error);
        }

        /// <summary>The on socket exception.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="failed">The failed.</param>
        public void OnSocketException(Socket endpoint, SocketException failed)
        {
            SocketExceptionEvent(endpoint, failed);
        }

        #endregion
    }
}