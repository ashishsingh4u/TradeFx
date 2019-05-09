//   ===================================================================================
//   <copyright file="TechieClientService.cs" company="TechieNotes">
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
//      The TechieClientService.cs file.
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

namespace TechieSocket.Net.Sockets.Client
{
    public class TechieClientService : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private readonly BlockingQueue<TechieEventArgs<byte[]>> _incomingQueue;

        private readonly BlockingQueue<TechieEventArgs<byte[]>> _outgoingQueue;

        private readonly ITechieSerializer _serializer;

        private readonly SocketClient _socketClient;

        #endregion

        #region Constructors and Destructors

        public TechieClientService(ITechieSerializer serializer)
        {
            _serializer = serializer;
            _incomingQueue = new BlockingQueue<TechieEventArgs<byte[]>>();
            _outgoingQueue = new BlockingQueue<TechieEventArgs<byte[]>>();
            _outgoingQueue.Subscribe(ProcessOutgoing, ProcessError);

            _socketClient = new SocketClient(4096)
                                {
                                    OnConnected = Connected,
                                    OnDisconnected = Disconnected,
                                    OnSent = OnSent,
                                    OnReceive = OnReceive
                                };

            _socketClient.Connect(IPAddress.Parse("127.0.0.1"), 4096);
        }

        public TechieClientService()
            : this(new ProtoBufSerializer())
        {
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _incomingQueue.Dispose();
            _outgoingQueue.Dispose();
            _socketClient.Dispose();
        }

        public void Publish<T>(T response) where T : ClientResponse
        {
            try
            {
                lock (_serializer)
                {
                    var data = _serializer.Serialize(response);
                    Logger.InfoFormat("Client:Serialize ByteCount {0}", data.Length);
                    _outgoingQueue.Publish(new TechieEventArgs<byte[]>(data));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void Subscribe<T>(Action<TechieEventArgs<T>> handler, Action<Exception> errorhandler)
            where T : ServerReponse
        {
            _incomingQueue.Subscribe(
                args =>
                    {
                        try
                        {
                            if (args != null)
                            {
                                lock (_serializer)
                                {
                                    Logger.InfoFormat("Client:Deserialize ByteCount {0}", args.Item.Length);
                                    var serverResponse = _serializer.Deserialize<T>(args.Item);
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

        private void Connected(Socket socket)
        {
            Logger.Info("Connected");
        }

        private void Disconnected(EndPoint endpoint)
        {
            Logger.Info("Disconnected");
        }

        private void OnReceive(SocketAsyncEventArgs e)
        {
            Logger.Info("OnReceive");
            _incomingQueue.Publish(new TechieEventArgs<byte[]>(e.CopyFromArgs()));
        }

        private void OnSent(SocketAsyncEventArgs e)
        {
            Logger.Info("OnSent");
        }

        private void ProcessError(Exception exception)
        {
            Logger.Error(exception);
        }

        private void ProcessOutgoing(EventArgs args)
        {
            try
            {
                var clientData = args as TechieEventArgs<byte[]>;
                if (clientData != null)
                {
                    _socketClient.Send(clientData.Item);
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