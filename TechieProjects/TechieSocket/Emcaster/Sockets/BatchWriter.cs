//  ===================================================================================
//  <copyright file="BatchWriter.cs" company="TechieNotes">
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
//     The BatchWriter.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Threading;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>
    ///     Writes bytes to a pending buffer. Thread safe for several writing threads.
    /// </summary>
    public class BatchWriter : IByteWriter
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(BatchWriter));

        #endregion

        #region Fields

        /// <summary>The _lock.</summary>
        private readonly object _lock = new object();

        /// <summary>The _target.</summary>
        private readonly IByteWriter _target;

        /// <summary>The _always sleep.</summary>
        private int _alwaysSleep = -1;

        /// <summary>The _flush buffer.</summary>
        private ByteBuffer _flushBuffer;

        /// <summary>The _flushed bytes.</summary>
        private long _flushedBytes;

        /// <summary>The _flushes.</summary>
        private long _flushes;

        /// <summary>The _min flush size.</summary>
        private int _minFlushSize = 1024 * 10;

        /// <summary>The _msg count.</summary>
        private int _msgCount;

        /// <summary>The _pending buffer.</summary>
        private ByteBuffer _pendingBuffer;

        /// <summary>The _print stats.</summary>
        private bool _printStats;

        /// <summary>The _running.</summary>
        private bool _running = true;

        /// <summary>The _sleep on min.</summary>
        private int _sleepOnMin = 10;

        /// <summary>The _sleep time.</summary>
        private long _sleepTime;

        /// <summary>The _stats interval.</summary>
        private int _statsInterval = 10;

        /// <summary>The _timer.</summary>
        private Timer _timer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BatchWriter"/> class.</summary>
        /// <param name="target">The target.</param>
        /// <param name="maxBufferSizeInBytes">The max buffer size in bytes.</param>
        public BatchWriter(IByteWriter target, int maxBufferSizeInBytes)
        {
            _target = target;
            _pendingBuffer = new ByteBuffer(maxBufferSizeInBytes);
            _flushBuffer = new ByteBuffer(maxBufferSizeInBytes);
            _minFlushSize = maxBufferSizeInBytes / 2;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the always sleep.</summary>
        public int AlwaysSleep
        {
            get
            {
                return _alwaysSleep;
            }

            set
            {
                this._alwaysSleep = value;
            }
        }

        /// <summary>Gets or sets the min flush size in bytes.</summary>
        public int MinFlushSizeInBytes
        {
            get
            {
                return _minFlushSize;
            }

            set
            {
                this._minFlushSize = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether print stats.</summary>
        public bool PrintStats
        {
            get
            {
                return _printStats;
            }

            set
            {
                _printStats = value;
            }
        }

        /// <summary>Gets or sets the sleep on min.</summary>
        public int SleepOnMin
        {
            get
            {
                return _sleepOnMin;
            }

            set
            {
                this._sleepOnMin = value;
            }
        }

        /// <summary>Gets or sets the stats interval in seconds.</summary>
        public int StatsIntervalInSeconds
        {
            get
            {
                return _statsInterval;
            }

            set
            {
                _statsInterval = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            log.Info(GetType().FullName + " Disposed");
            StopStats();
            lock (_lock)
            {
                _running = false;
                Monitor.PulseAll(_lock);
            }
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            WaitCallback callback = delegate { FlushRunner(); };
            ThreadPool.QueueUserWorkItem(callback);
            StartStats();
        }

        /// <summary>The start stats.</summary>
        public void StartStats()
        {
            if (_printStats)
            {
                TimerCallback timerCallBack = delegate { LogStats(); };
                _timer = new Timer(timerCallBack, null, _statsInterval * 1000, _statsInterval * 1000);
            }
        }

        /// <summary>The stop stats.</summary>
        public void StopStats()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        /// <summary>Add bytes to the buffer. If the buffer is full, then the thread waits for
        ///     the buffer to be flushed by the flushing thread. Thread Safe.</summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="waitMs">The wait Ms.</param>
        /// <returns>true if bytes are bufferred. false if the wait times out.</returns>
        public bool Write(byte[] buffer, int offset, int size, int waitMs)
        {
            lock (_lock)
            {
                while ((_pendingBuffer.Length + size) > _pendingBuffer.Capacity && _running)
                {
                    if (!Monitor.Wait(_lock, waitMs))
                    {
                        return false;
                    }
                }

                if (!_running)
                {
                    return false;
                }

                _pendingBuffer.Write(buffer, offset, size);
                if (_printStats)
                {
                    _msgCount++;
                }

                // other threads could be waiting to buffer Or the flush thread 
                // could be waiting to retry.
                Monitor.PulseAll(_lock);
                return true;
            }
        }

        #endregion

        #region Methods

        /// <summary>The flush buffer.</summary>
        internal void FlushBuffer()
        {
            lock (_lock)
            {
                while (_running && _pendingBuffer.Length == 0)
                {
                    Monitor.Wait(_lock);
                }

                if (!_running)
                {
                    return;
                }

                var swap = _flushBuffer;
                _flushBuffer = _pendingBuffer;
                _pendingBuffer = swap;

                // there may be many threads waiting to add to the buffer.
                Monitor.PulseAll(_lock);
            }

            long length = _flushBuffer.Length;
            if (length > 0)
            {
                try
                {
                    _flushBuffer.WriteTo(_target);
                }
                catch (Exception failed)
                {
                    log.Error("Async Flush Failed msg: " + failed.Message + " stack: " + failed.StackTrace);
                }

                _flushBuffer.Reset();
                if (_printStats)
                {
                    _flushes++;
                    _flushedBytes += length;
                }

                if (length < _minFlushSize)
                {
                    Sleep(_sleepOnMin);
                }
                else if (_alwaysSleep >= 0)
                {
                    Sleep(_alwaysSleep);
                }
            }
        }

        /// <summary>The flush runner.</summary>
        internal void FlushRunner()
        {
            log.Debug("Started Flush Thread for " + GetType().FullName);
            while (_running)
            {
                FlushBuffer();
            }
        }

        /// <summary>The log stats.</summary>
        private void LogStats()
        {
            double avgBytes = 0;
            if (_flushes > 0)
            {
                avgBytes = _flushedBytes / _flushes;
            }

            var avgMessages = _msgCount / _statsInterval;
            log.Info(
                "MsgAvg: " + avgMessages + " Flushes: " + _flushes + " Avg/Bytes: " + avgBytes + " Sleep(ms): "
                + _sleepTime);
            _msgCount = 0;
            _flushes = 0;
            _flushedBytes = 0;
            _sleepTime = 0;
        }

        /// <summary>The sleep.</summary>
        /// <param name="sleep">The sleep.</param>
        private void Sleep(int sleep)
        {
            Thread.Sleep(sleep);
            if (_printStats)
            {
                _sleepTime += sleep;
            }
        }

        #endregion
    }
}