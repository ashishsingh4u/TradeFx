using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TechieSocket.Net.Sockets.Common
{
    public class AsyncArgsPool : IDisposable
    {

        #region Fields

        private readonly Stack<SocketAsyncEventArgs> _pool;

        #endregion

        #region Constructors

        public AsyncArgsPool(int capacity)
        {
            _pool = new Stack<SocketAsyncEventArgs>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                Push(new SocketAsyncEventArgs());
            }
        }

        #endregion

        #region Services

        public void Push(SocketAsyncEventArgs item)
        {
            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (_pool)
            {
                return _pool.Pop();
            }
        }

        public int Count
        {
            get
            {
                lock (_pool)
                {
                    return _pool.Count;
                }
            }
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// Destructor of SocketAsyncEventArgsPool
        /// </summary>
        ~AsyncArgsPool()
        {
            Dispose();
        }

        /// <summary>
        /// IDisposable.Dispose for SocketAsyncEventArgsPool
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Disposer of this object.
        /// </summary>
        /// <param name="disposing">disposing option</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //TBD: Dispose managed resources
                    lock (_pool)
                    {
                        foreach (SocketAsyncEventArgs element in _pool)
                        {
                            element.Dispose();
                        }
                    }
                }

                //TBD: Release unmanaged resources
            }
            _disposed = true;
        }

        #endregion
    }
}
