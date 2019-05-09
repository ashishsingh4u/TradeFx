//  ===================================================================================
//  <copyright file="ITopicMessage.cs" company="TechieNotes">
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
//     The ITopicMessage.cs file.
//  </summary>
//  ===================================================================================

using System.Net;

namespace Emcaster.Topics
{
    /// <summary>The TopicMessage interface.</summary>
    public interface ITopicMessage
    {
        #region Public Properties

        /// <summary>Gets the end point.</summary>
        EndPoint EndPoint { get; }

        /// <summary>Gets the topic.</summary>
        string Topic { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The parse bytes.</summary>
        /// <returns>The <see cref="byte[]"/>.</returns>
        byte[] ParseBytes();

        /// <summary>The parse object.</summary>
        /// <returns>The <see cref="object"/>.</returns>
        object ParseObject();

        #endregion
    }
}