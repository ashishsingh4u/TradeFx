//  ===================================================================================
//  <copyright file="TopicMonitor.cs" company="TechieNotes">
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
//     The TopicMonitor.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Threading;

using log4net;

namespace Emcaster.Topics
{
    /// <summary>The topic monitor.</summary>
    public class TopicMonitor : IDisposable
    {
        #region Fields

        /// <summary>The _interval.</summary>
        private readonly int _interval;

        /// <summary>The log.</summary>
        private readonly ILog log;

        /// <summary>The _msg count.</summary>
        private long _msgCount;

        /// <summary>The _timer.</summary>
        private Timer _timer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TopicMonitor"/> class.</summary>
        /// <param name="logName">The log name.</param>
        /// <param name="intervalSeconds">The interval seconds.</param>
        public TopicMonitor(string logName, int intervalSeconds)
        {
            log = LogManager.GetLogger(logName);
            _interval = intervalSeconds;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        /// <summary>The on message.</summary>
        /// <param name="parser">The parser.</param>
        public void OnMessage(IMessageParser parser)
        {
            _msgCount++;
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            TimerCallback callback = delegate { OnTimer(); };
            _timer = new Timer(callback, null, 1000 * _interval, 1000 * _interval);
        }

        #endregion

        #region Methods

        /// <summary>The on timer.</summary>
        private void OnTimer()
        {
            double avg = _msgCount / _interval;
            log.Info("msg count: " + _msgCount + " avg/sec: " + avg);
            _msgCount = 0;
        }

        #endregion
    }
}