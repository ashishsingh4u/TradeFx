//   ===================================================================================
//   <copyright file="BufferManager.cs" company="TechieNotes">
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
//      The BufferManager.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

using log4net;

namespace TechieSocket.Net.Sockets.Common
{
    public class BufferManager : IDisposable
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private readonly int _bufferSize;

        private readonly int _numbytes;

        private byte[] _buffer;

        private int _currentIndex;

        private Stack<int> _freeIndexPool;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        public BufferManager(int numberOfBuffers, int bufferSize)
        {
            Disposed = false;
            _numbytes = numberOfBuffers * bufferSize;
            _bufferSize = bufferSize;

            _currentIndex = 0;
            _freeIndexPool = new Stack<int>();
            _buffer = new byte[_numbytes];
        }

        #endregion

        #endregion

        #region Services

        public void SetBuffer(SocketAsyncEventArgs args)
        {
            try
            {
                lock (_freeIndexPool)
                {
                    if (_freeIndexPool.Count > 0)
                    {
                        args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
                    }
                    else
                    {
                        args.SetBuffer(_buffer, _currentIndex, _bufferSize);
                        _currentIndex += _bufferSize;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            try
            {
                lock (_freeIndexPool)
                {
                    _freeIndexPool.Push(args.Offset);
                    args.SetBuffer(null, 0, 0);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public int FreeBufferCount
        {
            get
            {
                lock (_freeIndexPool)
                {
                    return ((_numbytes - _currentIndex) / _bufferSize) + _freeIndexPool.Count;
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Destructor of BufferManager
        /// </summary>
        ~BufferManager()
        {
            Dispose();
        }

        /// <summary>
        ///     IDisposable.Dispose for BufferManager
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
                    _buffer = null;
                    _freeIndexPool = null;
                }

                //TBD: Release unmanaged resources
            }
            Disposed = true;
        }

        #endregion
    }
}