//   ===================================================================================
//   <copyright file="SocketListener.cs" company="TechieNotes">
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
//   <date>28-05-2015</date>
//   <summary>
//      The SocketListener.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;

using log4net;

using TechieSocket.Net.Sockets.Common;

namespace TechieSocket.Net.Sockets.Server
{
    public class SocketListener : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        public Action<Socket> OnConnected;

        public Action<EndPoint> OnDisconnected;

        public Action<SocketAsyncEventArgs> OnReceived;

        public Action<SocketAsyncEventArgs> OnSent;

        public Action<Socket> OnStarted;

        public Action<EndPoint> OnStopped;

        private readonly int _backlog;

        private readonly int _bufferSize;

        private readonly IObservable<Connection> _connectionObservable;

        private readonly List<Connection> _connections;

        private readonly EndPoint _endPoint;

        private readonly BlockingCollection<byte[]> _outgoingQueue;

        private readonly IDisposable _outgoingQueueSubscription;

        private readonly int _poolSize;

        private SocketAsyncEventArgs _acceptArgs;

        private BufferManager _bufferManager;

        private Socket _listenSocket;

        private AsyncArgsPool _readwritePool;

        #endregion

        #region Constructors and Destructors

        public SocketListener(IPAddress address, int port, int backlog, int poolSize, int bufferSize)
        {
            _backlog = backlog;
            _poolSize = poolSize;
            _bufferSize = bufferSize;
            _endPoint = new IPEndPoint(address, port);

            _acceptArgs = new SocketAsyncEventArgs();
            _acceptArgs.Completed += AcceptCompleted;

            _readwritePool = new AsyncArgsPool(poolSize);
            _bufferManager = new BufferManager(poolSize, bufferSize);

            _connections = new List<Connection>();
            _connectionObservable = _connections.ToObservable(TaskPoolScheduler.Default);

            _outgoingQueue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            var outgoingQueueObservable = _outgoingQueue.GetConsumingEnumerable()
                .ToObservable(TaskPoolScheduler.Default);
            _outgoingQueueSubscription = outgoingQueueObservable.Subscribe(ProcessOutgoingData);
        }

        #endregion

        #region Public Methods and Operators

        public bool IsRunning()
        {
            return _listenSocket != null;
        }

        public void ProcessOutgoingData(byte[] data)
        {
            try
            {
                lock (_connections)
                {
                    _connectionObservable.ForEachAsync(connection => { connection.SendToClient(data); });
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void SendToClient(byte[] data)
        {
            _outgoingQueue.Add(data);
        }

        public void StartListener()
        {
            try
            {
                if (IsRunning())
                {
                    throw new InvalidOperationException("The Server is already running.");
                }

                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                                    {
                                        NoDelay = true
                                    };
                _listenSocket.Bind(_endPoint);
                _listenSocket.Listen(_backlog);

                if (OnStarted != null)
                {
                    OnStarted(_listenSocket);
                }

                StartAccept();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void StopListener()
        {
            try
            {
                if (_listenSocket == null)
                {
                    return;
                }
                _listenSocket.Close();
                _listenSocket = null;

                if (OnStopped != null)
                {
                    OnStopped(_endPoint);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion

        #region Methods

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.OperationAborted)
            {
                return;
            }

            if (e.SocketError == SocketError.Success)
            {
                //Checkout Buffer
                try
                {
                    var messageArgs = _readwritePool.Pop();
                    _bufferManager.SetBuffer(messageArgs);

                    var outgoingmessageArgs = _readwritePool.Pop();
                    _bufferManager.SetBuffer(outgoingmessageArgs);

                    _connections.Add(new Connection(
                            _bufferSize,
                            e.AcceptSocket,
                            messageArgs,
                            outgoingmessageArgs,
                            RemoveConnection,
                            OnDisconnected,
                            OnReceived,
                            OnSent,
                            connection =>
                                {
                                    lock (_connections)
                                    {
                                        _connections.Remove(connection);
                                    }
                                }));

                    if (OnConnected != null)
                    {
                        OnConnected(e.AcceptSocket);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
            }

            StartAccept();
        }

        private void RemoveConnection(SocketAsyncEventArgs messageArgs)
        {
            _bufferManager.FreeBuffer(messageArgs);
            _readwritePool.Push(messageArgs);
        }

        private void StartAccept()
        {
            try
            {
                if (_acceptArgs.AcceptSocket != null)
                {
                    _acceptArgs.AcceptSocket = null;
                }

                if (!_listenSocket.AcceptAsync(_acceptArgs))
                {
                    ProcessAccept(_acceptArgs);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        ~SocketListener()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        StopListener();
                        if (_acceptArgs != null)
                        {
                            _acceptArgs.Dispose();
                            _acceptArgs = null;
                        }
                        if (_readwritePool != null)
                        {
                            _readwritePool.Dispose();
                            _readwritePool = null;
                        }
                        if (_bufferManager != null)
                        {
                            _bufferManager.Dispose();
                            _bufferManager = null;
                        }
                        _outgoingQueueSubscription.Dispose();
                    }
                    _disposed = true;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion
    }
}