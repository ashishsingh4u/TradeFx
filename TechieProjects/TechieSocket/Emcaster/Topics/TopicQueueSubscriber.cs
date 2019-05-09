//  ===================================================================================
//  <copyright file="TopicQueueSubscriber.cs" company="TechieNotes">
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
//     The TopicQueueSubscriber.cs file.
//  </summary>
//  ===================================================================================

using System.Collections.Generic;
using System.Threading;

namespace Emcaster.Topics
{
    /// <summary>The topic queue subscriber.</summary>
    public class TopicQueueSubscriber
    {
        #region Fields

        /// <summary>The _lock.</summary>
        private readonly object _lock = new object();

        /// <summary>The _max size.</summary>
        private readonly int _maxSize;

        /// <summary>The _topic.</summary>
        private readonly ITopicSubscriber _topic;

        /// <summary>The _msg queue.</summary>
        private List<ITopicMessage> _msgQueue = new List<ITopicMessage>();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TopicQueueSubscriber"/> class.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="maxSize">The max size.</param>
        public TopicQueueSubscriber(ITopicSubscriber topic, int maxSize)
        {
            _topic = topic;
            _maxSize = maxSize;
        }

        #endregion

        #region Public Events

        /// <summary>The discard event.</summary>
        public event OnTopicMessage DiscardEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets all queued messages. If no messages are available, the thread will wait the
        ///     provided wait time for a message.</summary>
        /// <param name="waitTimeMs"></param>
        /// <returns>The <see cref="IList"/>.</returns>
        public IList<ITopicMessage> Dequeue(int waitTimeMs)
        {
            lock (_lock)
            {
                if (_msgQueue.Count > 0)
                {
                    IList<ITopicMessage> toReturn = _msgQueue;
                    _msgQueue = new List<ITopicMessage>();
                    return toReturn;
                }
                else
                {
                    Monitor.Wait(_lock, waitTimeMs);
                    IList<ITopicMessage> toReturn = _msgQueue;
                    _msgQueue = new List<ITopicMessage>();
                    return toReturn;
                }
            }
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            _topic.TopicMessageEvent += OnMessage;
        }

        /// <summary>The stop.</summary>
        public void Stop()
        {
            _topic.TopicMessageEvent -= OnMessage;
        }

        #endregion

        #region Methods

        /// <summary>The on message.</summary>
        /// <param name="parser">The parser.</param>
        private void OnMessage(IMessageParser parser)
        {
            var bytes = new ByteMessageParser(parser.Topic, parser.ParseBytes(), parser.EndPoint);
            var discard = false;
            lock (_lock)
            {
                if (_msgQueue.Count >= _maxSize)
                {
                    discard = true;
                }
                else
                {
                    _msgQueue.Add(bytes);
                    Monitor.Pulse(_lock);
                }
            }

            var discardMsg = DiscardEvent;
            if (discard && discardMsg != null)
            {
                discardMsg(bytes);
            }
        }

        #endregion
    }
}