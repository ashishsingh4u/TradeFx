//  ===================================================================================
//  <copyright file="ISourceReader.cs" company="TechieNotes">
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
//     The ISourceReader.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    /// <summary>The on socket exception.</summary>
    /// <param name="socket">The socket.</param>
    /// <param name="socketExc">The socket exc.</param>
    public delegate void OnSocketException(Socket socket, SocketException socketExc);

    /// <summary>The on exception.</summary>
    /// <param name="socket">The socket.</param>
    /// <param name="socketFailed">The socket failed.</param>
    public delegate void OnException(Socket socket, Exception socketFailed);

    /// <summary>The SourceReader interface.</summary>
    public interface ISourceReader : IPacketEvent
    {
        #region Public Events

        /// <summary>The exception event.</summary>
        event OnException ExceptionEvent;

        /// <summary>The socket exception event.</summary>
        event OnSocketException SocketExceptionEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>The accept socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="acceptor">The acceptor.</param>
        void AcceptSocket(Socket socket, IAcceptor acceptor);

        #endregion
    }
}