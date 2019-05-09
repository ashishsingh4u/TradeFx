//   ===================================================================================
//   <copyright file="SocketClient.cs" company="TechieNotes">
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
//      The SocketClient.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using log4net;

namespace TechieSocket.Net.Sockets.Client
{
    public class SocketClient : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        public Action<Socket> OnConnected;

        public Action<EndPoint> OnDisconnected;

        public Action<SocketAsyncEventArgs> OnReceive;

        public Action<SocketAsyncEventArgs> OnSent;

        private readonly AutoResetEvent _autoResetEvent;

        private SocketAsyncEventArgs _connectArgs;

        private EndPoint _endPoint;

        private SocketAsyncEventArgs _receiveArgs;

        private SocketAsyncEventArgs _sendArgs;

        private Socket _socket;

        #endregion

        #region Constructors and Destructors

        public SocketClient(int bufferSize)
        {
            Disposed = false;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                          {
                              NoDelay = true
                          };

            _connectArgs = new SocketAsyncEventArgs();
            _connectArgs.Completed += ConnectCompleted;
            _connectArgs.UserToken = _socket;

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += SendCompleted;
            _sendArgs.UserToken = _socket;

            _receiveArgs = new SocketAsyncEventArgs();
            _receiveArgs.Completed += ReceiveCompleted;
            _receiveArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
            _receiveArgs.UserToken = _socket;

            _autoResetEvent = new AutoResetEvent(true);
        }

        #endregion

        #region Public Methods and Operators

        public void Connect(IPAddress address, int port)
        {
            try
            {
                _endPoint = new IPEndPoint(address, port);
                _connectArgs.RemoteEndPoint = _endPoint;
                if (!_socket.ConnectAsync(_connectArgs))
                {
                    ConnectCompleted(this, _connectArgs);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (IsConnected())
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                    _connectArgs = null;
                }

                if (OnDisconnected != null)
                {
                    OnDisconnected(_endPoint);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public bool IsConnected()
        {
            var result = false;

            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    result = true;
                }
            }

            return result;
        }

        public void Send(byte[] buffer)
        {
            try
            {
                _autoResetEvent.WaitOne();
                if (!IsConnected())
                {
                    throw new SocketException();
                }

                if (buffer.Length <= 0)
                {
                    return;
                }

                _sendArgs.SetBuffer(buffer, 0, buffer.Length);
                if (!_socket.SendAsync(_sendArgs))
                {
                    SendCompleted(this, _sendArgs);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion

        #region Methods

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    StartReceive(_receiveArgs);

                    if (OnConnected != null)
                    {
                        OnConnected(_socket);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred == 0)
                {
                    return;
                }

                if (OnReceive != null)
                {
                    OnReceive(e);
                }

                StartReceive(e);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (OnSent != null)
                {
                    OnSent(e);
                }
                _autoResetEvent.Set();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (!IsConnected())
                {
                    throw new SocketException();
                }

                if (!_socket.ReceiveAsync(e))
                {
                    ReceiveCompleted(this, e);
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
        ///     Destructor of SocketClient
        /// </summary>
        ~SocketClient()
        {
            Dispose();
        }

        /// <summary>
        ///     IDisposable.Dispose for SocketClient
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
                    Disconnect();

                    if (_connectArgs != null)
                    {
                        _connectArgs.Dispose();
                        _connectArgs = null;
                    }

                    if (_sendArgs != null)
                    {
                        _sendArgs.Dispose();
                        _sendArgs = null;
                    }

                    if (_receiveArgs != null)
                    {
                        _receiveArgs.Dispose();
                        _receiveArgs = null;
                    }
                }

                //TBD: Release unmanaged resources
            }
            Disposed = true;
        }

        #endregion
    }
}