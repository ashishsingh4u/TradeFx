//   ===================================================================================
//   <copyright file="AmpleTcpServer.cs" company="TechieNotes">
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
//   <date>18-05-2015</date>
//   <summary>
//      The AmpleTcpServer.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TechieSocket.Net.Sockets.Server
{
    public class IpEndPointEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public IpEndPointEventArgs(IPEndPoint ipEndPoint)
        {
            IpEndPoint = ipEndPoint;
        }

        #endregion

        #region Public Properties

        public IPEndPoint IpEndPoint { get; private set; }

        #endregion
    }

    public class DataReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public DataReceivedEventArgs(byte[] data, IPEndPoint ipEndPoint)
        {
            Data = data;
            IpEndPoint = ipEndPoint;
        }

        #endregion

        #region Public Properties

        public byte[] Data { get; private set; }

        public IPEndPoint IpEndPoint { get; private set; }

        #endregion
    }

    /// <summary>
    ///     TcpListner wrapper
    ///     Encapsulates asyncronous communications using TCP/IP.
    /// </summary>
    public sealed class TcpServer : IDisposable
    {
        #region Constants

        private const int SocketBufferSize = 1024;

        #endregion

        #region Fields

        private readonly Dictionary<IPEndPoint, Socket> _connectedSockets;

        private readonly object _connectedSocketsSyncHandle = new object();

        private readonly object _syncHandle = new object();

        private readonly TcpListener _tcpServer;

        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        //---------------------------------------------------------------------- 
        //Construction, Destruction 
        //---------------------------------------------------------------------- 
        /// <summary>
        ///     Creating server socket
        /// </summary>
        /// <param name="port">Server port number</param>
        public TcpServer(int port)
        {
            _connectedSockets = new Dictionary<IPEndPoint, Socket>();
            _tcpServer = new TcpListener(IPAddress.Any, port);
            _tcpServer.Start();
            _tcpServer.BeginAcceptSocket(EndAcceptSocket, _tcpServer);
        }

        ~TcpServer()
        {
            DisposeImpl(false);
        }

        #endregion

        #region Public Events

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        //---------------------------------------------------------------------- 
        //Events 
        //---------------------------------------------------------------------- 
        public event EventHandler<IpEndPointEventArgs> SocketConnected;

        public event EventHandler<IpEndPointEventArgs> SocketDisconnected;

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            DisposeImpl(true);
        }

        //---------------------------------------------------------------------- 
        //Public Methods 
        //---------------------------------------------------------------------- 

        public void SendData(byte[] data, IPEndPoint endPoint)
        {
            Socket sock;
            lock (_syncHandle)
            {
                if (!_connectedSockets.ContainsKey(endPoint))
                {
                    return;
                }
                sock = _connectedSockets[endPoint];
            }
            sock.Send(data);
        }

        #endregion

        //---------------------------------------------------------------------- 
        //Private Functions 
        //---------------------------------------------------------------------- 

        #region Private Functions

        //Обработка нового соединения 
        private void Connected(Socket socket)
        {
            var endPoint = (IPEndPoint)socket.RemoteEndPoint;

            lock (_connectedSocketsSyncHandle)
            {
                if (_connectedSockets.ContainsKey(endPoint))
                {
                    //theLog.Log.DebugFormat("TcpServer.Connected: Socket already connected! Removing from local storage! EndPoint: {0}", endPoint);
                    _connectedSockets[endPoint].Close();
                }

                SetDesiredKeepAlive(socket);
                _connectedSockets[endPoint] = socket;
            }

            OnSocketConnected(endPoint);
        }

        private static void SetDesiredKeepAlive(Socket socket)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            const uint Time = 10000;
            const uint Interval = 20000;
            SetKeepAlive(socket, true, Time, Interval);
        }

        private static void SetKeepAlive(Socket s, bool on, uint time, uint interval)
        {
            /* the native structure 
            struct tcp_keepalive { 
            ULONG onoff; 
            ULONG keepalivetime; 
            ULONG keepaliveinterval; 
            }; 
            */

            // marshal the equivalent of the native structure into a byte array 
            uint dummy = 0;
            var inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(on ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes(time).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes(interval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            // of course there are other ways to marshal up this byte array, this is just one way 

            // call WSAIoctl via IOControl 
            s.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        //socket disconnected handler 
        private void Disconnect(Socket socket)
        {
            var endPoint = (IPEndPoint)socket.RemoteEndPoint;

            lock (_connectedSocketsSyncHandle)
            {
                _connectedSockets.Remove(endPoint);
            }

            socket.Close();

            OnSocketDisconnected(endPoint);
        }

        private void ReceiveData(byte[] data, IPEndPoint endPoint)
        {
            OnDataReceived(data, endPoint);
        }

        private void EndAcceptSocket(IAsyncResult asyncResult)
        {
            var lister = (TcpListener)asyncResult.AsyncState;
            //theLog.Log.Debug("TcpServer.EndAcceptSocket"); 
            if (_disposed)
            {
                //theLog.Log.Debug("TcpServer.EndAcceptSocket: tcp server already disposed!"); 
                return;
            }

            try
            {
                Socket sock;
                try
                {
                    sock = lister.EndAcceptSocket(asyncResult);
                    //theLog.Log.DebugFormat("TcpServer.EndAcceptSocket: remote end point: {0}", sock.RemoteEndPoint); 
                    Connected(sock);
                }
                finally
                {
                    //EndAcceptSocket can failes, but in any case we want to accept new connections 
                    lister.BeginAcceptSocket(EndAcceptSocket, lister);
                }

                //we can use this only from .net framework 2.0 SP1 and higher 
                var e = new SocketAsyncEventArgs();
                e.Completed += ReceiveCompleted;
                e.SetBuffer(new byte[SocketBufferSize], 0, SocketBufferSize);
                BeginReceiveAsync(sock, e);
            }
            catch (SocketException)
            {
                //theLog.Log.Error("TcpServer.EndAcceptSocket: failes!", ex); 
            }
            catch (Exception)
            {
                //theLog.Log.Error("TcpServer.EndAcceptSocket: failes!", ex); 
            }
        }

        private void BeginReceiveAsync(Socket sock, SocketAsyncEventArgs e)
        {
            if (!sock.ReceiveAsync(e))
            {
                //IO operation finished syncronously 
                //handle received data 
                ReceiveCompleted(sock, e);
            } //IO operation finished syncronously 
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            var sock = (Socket)sender;
            if (!sock.Connected)
            {
                Disconnect(sock);
            }
            try
            {
                var size = e.BytesTransferred;
                if (size == 0)
                {
                    //this implementation based on IO Completion ports, and in this case 
                    //receiving zero bytes mean socket disconnection 
                    Disconnect(sock);
                }
                else
                {
                    var buf = new byte[size];
                    Array.Copy(e.Buffer, buf, size);
                    ReceiveData(buf, (IPEndPoint)sock.RemoteEndPoint);
                    BeginReceiveAsync(sock, e);
                }
            }
            catch (SocketException)
            {
                //We can't truly handle this excpetion here, but unhandled 
                //exception caused process termination. 
                //You can add new event to notify observer 
                // theLog.Log.Error("TcpServer: receive data error!", ex);
            }
            catch (Exception)
            {
                //theLog.Log.Error("TcpServer: receive data error!", ex);
            }
        }

        private void DisposeImpl(bool manualDispose)
        {
            if (manualDispose)
            {
                //We should manually close all connected sockets 
                Exception error = null;
                try
                {
                    if (_tcpServer != null)
                    {
                        _disposed = true;
                        _tcpServer.Stop();
                    }
                }
                catch (Exception ex)
                {
                    //theLog.Log.Error("TcpServer: tcpServer.Stop() failes!", ex);
                    error = ex;
                }

                try
                {
                    lock (_syncHandle)
                    {
                        foreach (var sock in _connectedSockets.Values)
                        {
                            sock.Close();
                        }
                    }
                }
                catch (SocketException ex)
                {
                    //During one socket disconnected we can faced exception 
                    //theLog.Log.Error("TcpServer: close accepted socket failes!", ex);
                    error = ex;
                }
                if (error != null)
                {
                    throw error;
                }
            }
        }

        private void OnSocketConnected(IPEndPoint ipEndPoint)
        {
            var handler = SocketConnected;
            if (handler != null)
            {
                handler(this, new IpEndPointEventArgs(ipEndPoint));
            }
        }

        private void OnSocketDisconnected(IPEndPoint ipEndPoint)
        {
            var handler = SocketDisconnected;
            if (handler != null)
            {
                handler(this, new IpEndPointEventArgs(ipEndPoint));
            }
        }

        private void OnDataReceived(byte[] data, IPEndPoint ipEndPoint)
        {
            var handler = DataReceived;
            if (handler != null)
            {
                handler(this, new DataReceivedEventArgs(data, ipEndPoint));
            }
        }

        #endregion Private Functions
    }
}