//  ===================================================================================
//  <copyright file="AsyncReader.cs" company="TechieNotes">
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
//     The AsyncReader.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The async reader.</summary>
    public class AsyncReader
    {
        #region Fields

        /// <summary>The _acceptor.</summary>
        private readonly IAcceptor _acceptor;

        /// <summary>The _buffer.</summary>
        private readonly byte[] _buffer;

        /// <summary>The _dipose lock.</summary>
        private readonly object _diposeLock = new object();

        /// <summary>The _endpoint.</summary>
        private readonly EndPoint _endpoint;

        /// <summary>The _error handler.</summary>
        private readonly ISocketErrorHandler _errorHandler;

        /// <summary>The _log.</summary>
        private readonly ILog _log;

        /// <summary>The _parser.</summary>
        private readonly IByteParser _parser;

        /// <summary>The _socket.</summary>
        private readonly Socket _socket;

        /// <summary>The _running.</summary>
        private bool _running = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AsyncReader"/> class.</summary>
        /// <param name="parser">The parser.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="acceptor">The acceptor.</param>
        /// <param name="errorHandler">The error handler.</param>
        /// <param name="socket">The socket.</param>
        public AsyncReader(
            IByteParser parser, byte[] buffer, IAcceptor acceptor, ISocketErrorHandler errorHandler, Socket socket)
        {
            _parser = parser;
            _buffer = buffer;
            _acceptor = acceptor;
            _errorHandler = errorHandler;
            _socket = socket;
            _endpoint = socket.RemoteEndPoint;
            _log = LogManager.GetLogger(_endpoint.ToString());
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The begin receive.</summary>
        public void BeginReceive()
        {
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReadBytes, null);
        }

        /// <summary>The close.</summary>
        public void Close()
        {
            AttemptClose();
        }

        #endregion

        #region Methods

        /// <summary>The attempt close.</summary>
        private void AttemptClose()
        {
            lock (_diposeLock)
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
                        _log.Error("Close Failed", failed);
                    }
                }
            }
        }

        /// <summary>The on read bytes.</summary>
        /// <param name="ar">The ar.</param>
        private void OnReadBytes(IAsyncResult ar)
        {
            if (!_acceptor.IsRunning || !_running)
            {
                AttemptClose();
                return;
            }

            try
            {
                var read = _socket.EndReceive(ar);
                if (read > 0)
                {
                    _parser.OnBytes(_endpoint, _buffer, 0, read);
                    BeginReceive();
                }
                else
                {
                    _log.Info("End Read");
                }
            }
            catch (SocketException socketExc)
            {
                _errorHandler.OnSocketException(_socket, socketExc);
                AttemptClose();
            }
            catch (Exception failed)
            {
                _errorHandler.OnException(_socket, failed);
                AttemptClose();
            }
        }

        #endregion
    }
}