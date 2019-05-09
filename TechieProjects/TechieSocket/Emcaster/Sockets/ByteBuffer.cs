//  ===================================================================================
//  <copyright file="ByteBuffer.cs" company="TechieNotes">
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
//     The ByteBuffer.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    /// <summary>The byte buffer.</summary>
    public class ByteBuffer
    {
        #region Fields

        /// <summary>The _buffer.</summary>
        private readonly byte[] _buffer;

        /// <summary>The _position.</summary>
        private int _position;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ByteBuffer"/> class.</summary>
        /// <param name="size">The size.</param>
        public ByteBuffer(int size)
        {
            _buffer = new byte[size];
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the capacity.</summary>
        public int Capacity
        {
            get
            {
                return _buffer.Length;
            }
        }

        /// <summary>Gets the length.</summary>
        public int Length
        {
            get
            {
                return _position;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The reset.</summary>
        public void Reset()
        {
            _position = 0;
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public void Write(byte[] data, int offset, int length)
        {
            Array.Copy(data, offset, _buffer, _position, length);
            _position += length;
        }

        /// <summary>The write to.</summary>
        /// <param name="writer">The writer.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool WriteTo(IByteWriter writer)
        {
            return writer.Write(_buffer, 0, _position, 0);
        }

        /// <summary>The write to.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int WriteTo(Socket socket, SocketFlags flags)
        {
            return socket.Send(_buffer, 0, _position, flags);
        }

        #endregion
    }
}