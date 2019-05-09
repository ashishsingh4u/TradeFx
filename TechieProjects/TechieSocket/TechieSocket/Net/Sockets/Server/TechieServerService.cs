//   ===================================================================================
//   <copyright file="TechieServerService.cs" company="TechieNotes">
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
//   <date>26-05-2015</date>
//   <summary>
//      The TechieServerService.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using log4net;

using TechieSocket.Net.Common;
using TechieSocket.Net.Sockets.Common;
using TechieSocket.Net.Sockets.Response;
using TechieSocket.Net.Sockets.Serializers;

namespace TechieSocket.Net.Sockets.Server
{
    public class TechieServerService : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private readonly BlockingQueue<TechieEventArgs<byte[]>> _incomingQueue;

        private readonly BlockingQueue<TechieEventArgs<byte[]>> _outgoingQueue;

        private readonly ITechieSerializer _serializer;

        private readonly SocketListener _socketListener;

        #endregion

        #region Constructors and Destructors

        public TechieServerService(ITechieSerializer serializer)
        {
            _serializer = serializer;
            _incomingQueue = new BlockingQueue<TechieEventArgs<byte[]>>();
            _outgoingQueue = new BlockingQueue<TechieEventArgs<byte[]>>();

            _socketListener = new SocketListener(IPAddress.Parse("127.0.0.1"), 4096, 10, 2000, 4096)
                                  {
                                      OnStarted = OnStarted,
                                      OnStopped = OnStopped,
                                      OnConnected = OnConnected,
                                      OnDisconnected = OnDisconnected,
                                      OnReceived = OnReceived,
                                      OnSent = OnSent
                                  };

            _outgoingQueue.Subscribe(ProcessOutgoing, ProcessError);
            _socketListener.StartListener();
        }

        public TechieServerService()
            : this(new ProtoBufSerializer())
        {
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _incomingQueue.Dispose();
            _outgoingQueue.Dispose();
            _socketListener.Dispose();
        }

        public void Publish<T>(T response) where T : ServerReponse
        {
            try
            {
                lock (_serializer)
                {
                    var data = _serializer.Serialize(response);
                    Logger.InfoFormat("Serialize:Serialize ByteCount {0}", data.Length);
                    _outgoingQueue.Publish(new TechieEventArgs<byte[]>(data));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void StopListener()
        {
            Logger.Info("StopListener");
            _socketListener.StopListener();
        }

        public void Subscribe<T>(Action<TechieEventArgs<T>> handler, Action<Exception> errorhandler)
            where T : ClientResponse
        {
            _incomingQueue.Subscribe(
                args =>
                    {
                        try
                        {
                            var techieEventArgs = args;
                            if (techieEventArgs != null)
                            {
                                lock (_serializer)
                                {
                                    var serverResponse = _serializer.Deserialize<T>(techieEventArgs.Item);
                                    Logger.InfoFormat("Server:Deserialize ByteCount {0}", techieEventArgs.Item.Length);
                                    handler(new TechieEventArgs<T>(serverResponse));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception);
                            errorhandler(exception);
                        }
                    },
                errorhandler);
        }

        #endregion

        #region Methods

        private void OnConnected(Socket socket)
        {
            Logger.Info("OnConnected");
        }

        private void OnDisconnected(EndPoint endpoint)
        {
            Logger.Info("OnDisconnected");
        }

        private void OnReceived(SocketAsyncEventArgs e)
        {
            _incomingQueue.Publish(new TechieEventArgs<byte[]>(e.CopyFromArgs()));
            Logger.Info("OnReceived");
        }

        private void OnSent(SocketAsyncEventArgs e)
        {
            Logger.Info("OnSent");
        }

        private void OnStarted(Socket socket)
        {
            Logger.Info("OnStarted");
        }

        private void OnStopped(EndPoint endPoint)
        {
            Logger.Info("OnStopped");
        }

        private void ProcessError(Exception exception)
        {
            Logger.Error(exception);
        }

        private void ProcessOutgoing(EventArgs args)
        {
            try
            {
                var serverData = args as TechieEventArgs<byte[]>;
                if (serverData != null)
                {
                    _socketListener.SendToClient(serverData.Item);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                ProcessError(exception);
            }
        }

        #endregion
    }
}