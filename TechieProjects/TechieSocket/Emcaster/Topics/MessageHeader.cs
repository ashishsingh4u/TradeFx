//  ===================================================================================
//  <copyright file="MessageHeader.cs" company="TechieNotes">
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
//     The MessageHeader.cs file.
//  </summary>
//  ===================================================================================
namespace Emcaster.Topics
{
    /// <summary>The message header.</summary>
    public struct MessageHeader
    {
        #region Fields

        /// <summary>The _body size.</summary>
        private readonly int _bodySize;

        /// <summary>The _topic size.</summary>
        private readonly int _topicSize;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MessageHeader"/> struct.</summary>
        /// <param name="topicSize">The topic size.</param>
        /// <param name="bodySize">The body size.</param>
        public MessageHeader(int topicSize, int bodySize)
        {
            _topicSize = topicSize;
            _bodySize = bodySize;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the body size.</summary>
        public int BodySize
        {
            get
            {
                return _bodySize;
            }
        }

        /// <summary>Gets the topic size.</summary>
        public int TopicSize
        {
            get
            {
                return _topicSize;
            }
        }

        /// <summary>Gets the total size.</summary>
        public int TotalSize
        {
            get
            {
                return _topicSize + _bodySize;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The write to buffer.</summary>
        /// <param name="allData">The all data.</param>
        public unsafe void WriteToBuffer(byte[] allData)
        {
            fixed (byte* pData = allData)
            {
                *((MessageHeader*)pData) = this;
            }
        }

        #endregion
    }
}