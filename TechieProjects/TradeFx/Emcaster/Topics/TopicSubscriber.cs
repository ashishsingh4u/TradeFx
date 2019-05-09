//  ===================================================================================
//  <copyright file="TopicSubscriber.cs" company="TechieNotes">
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
//     The TopicSubscriber.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Text.RegularExpressions;

namespace Emcaster.Topics
{
    /// <summary>The topic subscriber.</summary>
    public class TopicSubscriber : IDisposable, ITopicSubscriber
    {
        #region Fields

        /// <summary>The _msg event.</summary>
        private readonly IMessageEvent _msgEvent;

        /// <summary>The _regex.</summary>
        private readonly Regex _regex;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TopicSubscriber"/> class.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="msgEvent">The msg event.</param>
        public TopicSubscriber(string topic, IMessageEvent msgEvent)
        {
            _regex = new Regex(topic, RegexOptions.Compiled);
            _msgEvent = msgEvent;
        }

        #endregion

        #region Public Events

        /// <summary>The topic message event.</summary>
        public event OnTopicMessage TopicMessageEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _msgEvent.MessageEvent -= OnTopicMessage;
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            _msgEvent.MessageEvent += OnTopicMessage;
        }

        /// <summary>The stop.</summary>
        public void Stop()
        {
            Dispose();
        }

        #endregion

        #region Methods

        /// <summary>The on topic message.</summary>
        /// <param name="parser">The parser.</param>
        private void OnTopicMessage(IMessageParser parser)
        {
            var msg = TopicMessageEvent;
            if (msg != null)
            {
                var topic = parser.Topic;
                if (_regex.IsMatch(parser.Topic))
                {
                    msg(parser);
                }
            }
        }

        #endregion
    }
}