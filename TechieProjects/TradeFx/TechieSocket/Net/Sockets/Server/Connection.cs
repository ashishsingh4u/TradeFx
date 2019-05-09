//   ===================================================================================
//   <copyright file="Connection.cs" company="TechieNotes">
//   ===================================================================================
//    TechieNotes Utilities & Best Practices
//    Samples and Guidelines for Winform & ASP.net development
//   ===================================================================================
//    Copyright (c) TechieNotes.  All rights reserved.
//    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//    OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//    LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//    FITNESS FOR A PARTICULAR PURPOSE.
//   ===================================================================================
//    The example companies, organizations, products, domain names,
//    e-mail addresses, logos, people, places, and events depicted
//    herein are fictitious.  No association with any real company,
//    organization, product, domain name, email address, logo, person,
//    places, or events is intended or should be inferred.
//   ===================================================================================
//   </copyright>
//   <author>Ashish Singh</author>
//   <email>mailto:ashishsingh4u@gmail.com</email>
//   <date>27-05-2015</date>
//   <summary>
//      The Connection.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using log4net;

using TechieSocket.Net.Sockets.Common;

namespace TechieSocket.Net.Sockets.Server
{
    public class Connection : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private readonly AutoResetEvent _autoResetEvent;

        private readonly int _bufferSize;

        private readonly EndPoint _endPoint;

        private readonly SocketAsyncEventArgs _messageArgs;

        private readonly int _offset;

        private readonly int _receiveMessageOffset;

        private readonly Action<Connection> _onClosed;

        private readonly Action<EndPoint> _onDisconnected;

        private readonly Action<SocketAsyncEventArgs> _onReceived;

        private readonly Action<SocketAsyncEventArgs> _onSent;

        private readonly SocketAsyncEventArgs _outgoingmessageArgs;

        private readonly Action<SocketAsyncEventArgs> _removeConnection;

        private Socket _socket;

        #endregion

        #region Constructors and Destructors

        public Connection(
            int bufferSize,
            Socket socket,
            SocketAsyncEventArgs messageArgs,
            SocketAsyncEventArgs outgoingmessageArgs,
            Action<SocketAsyncEventArgs> removeConnection,
            Action<EndPoint> onDisconnected,
            Action<SocketAsyncEventArgs> onReceived,
            Action<SocketAsyncEventArgs> onSent,
            Action<Connection> onClosed)
        {
            Disposed = false;
            _bufferSize = bufferSize;
            _socket = socket;
            _endPoint = socket.RemoteEndPoint;
            _offset = messageArgs.Offset;
            _receiveMessageOffset = outgoingmessageArgs.Offset;
            _removeConnection = removeConnection;

            _onDisconnected = onDisconnected;
            _onReceived = onReceived;
            _onSent = onSent;
            _onClosed = onClosed;

            _messageArgs = messageArgs;
            _messageArgs.Completed += IncomingMessageCompleted;
            _messageArgs.UserToken = socket;

            _outgoingmessageArgs = outgoingmessageArgs;
            _outgoingmessageArgs.Completed += OutgoingMessageCompleted;
            _outgoingmessageArgs.UserToken = socket;

            _autoResetEvent = new AutoResetEvent(true);

            StartReceive();
        }

        #endregion

        #region Public Methods and Operators

        public void SendToClient(byte[] data)
        {
            try
            {
                _autoResetEvent.WaitOne();
                _outgoingmessageArgs.CopyToArgs(data);

                if (!_socket.SendAsync(_outgoingmessageArgs))
                {
                    ProcessOutgointMessageSent(_outgoingmessageArgs);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion

        #region Methods

        private void CloseClientSocket()
        {
            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (SocketException socketExcept)
                    {
                        throw new SocketException(socketExcept.ErrorCode);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception);
                    }
                    finally
                    {
                        _socket.Close();
                        _socket = null;
                    }
                }

                _messageArgs.Completed -= IncomingMessageCompleted;
                _outgoingmessageArgs.Completed -= OutgoingMessageCompleted;

                _removeConnection(_messageArgs);

                if (_onDisconnected != null)
                {
                    _onDisconnected(_endPoint);
                }

                if (_onClosed != null)
                {
                    _onClosed(this);
                }
                _autoResetEvent.Dispose();
            }
        }

        private void IncomingMessageCompleted(object sender, SocketAsyncEventArgs e)
        {
            IncomingProcessMessage(e);
        }

        private void IncomingProcessMessage(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //implementing messaging operation
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Receive:
                            ProcessReceive(e);
                            break;
                        case SocketAsyncOperation.Send:
                            ProcessSent(e);
                            break;
                        default:
                            throw new ArgumentException(
                                "The last operation completed on the socket was not a receive or send");
                    }
                }
                else
                {
                    CloseClientSocket();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void OutgoingMessageCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //implementing messaging operation
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Send:
                            ProcessOutgointMessageSent(e);
                            break;
                        default:
                            throw new ArgumentException(
                                "The last operation completed on the socket was not a receive or send");
                    }
                }
                else
                {
                    CloseClientSocket();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void ProcessOutgointMessageSent(SocketAsyncEventArgs e)
        {
            try
            {
                if (_onSent != null)
                {
                    _onSent(e);
                }

                e.SetBuffer(_receiveMessageOffset, _bufferSize);

                _autoResetEvent.Set();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (_onReceived != null)
                {
                    _onReceived(e);
                }

                StartReceive();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void ProcessSent(SocketAsyncEventArgs e)
        {
            try
            {
                if (_onSent != null)
                {
                    _onSent(e);
                }

                e.SetBuffer(_offset, _bufferSize);
                StartReceive();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void StartReceive()
        {
            try
            {
                if (_socket.Connected)
                {
                    if (!_socket.ReceiveAsync(_messageArgs))
                    {
                        IncomingProcessMessage(_messageArgs);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Destructor of ServerConnection
        /// </summary>
        ~Connection()
        {
            Dispose();
        }

        /// <summary>
        ///     IDisposable.Dispose for ServerConnection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Track whether Dispose has been called.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        ///     Disposer of this object.
        /// </summary>
        /// <param name="disposing">disposing option</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    //TBD: Dispose managed resources
                    CloseClientSocket();
                }
                //TBD: Release unmanaged resources
            }
            Disposed = true;
        }

        #endregion
    }
}