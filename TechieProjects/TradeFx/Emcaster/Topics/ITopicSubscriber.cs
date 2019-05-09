//  ===================================================================================
//  <copyright file="ITopicSubscriber.cs" company="TechieNotes">
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
//     The ITopicSubscriber.cs file.
//  </summary>
//  ===================================================================================
namespace Emcaster.Topics
{
    /// <summary>The on topic message.</summary>
    /// <param name="parser">The parser.</param>
    public delegate void OnTopicMessage(IMessageParser parser);

    /// <summary>The TopicSubscriber interface.</summary>
    public interface ITopicSubscriber
    {
        #region Public Events

        /// <summary>The topic message event.</summary>
        event OnTopicMessage TopicMessageEvent;

        #endregion
    }
}