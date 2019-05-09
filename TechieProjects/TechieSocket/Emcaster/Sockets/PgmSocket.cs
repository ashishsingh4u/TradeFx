//  ===================================================================================
//  <copyright file="PgmSocket.cs" company="TechieNotes">
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
//  <date>27-01-2013</date>
//  <summary>
//     The PgmSocket.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using log4net;

namespace Emcaster.Sockets
{
    /// <summary>The pgm socket.</summary>
    public class PgmSocket : Socket
    {
        #region Static Fields

        /// <summary>The pg m_ level.</summary>
        public static readonly SocketOptionLevel PGM_LEVEL = (SocketOptionLevel)113;

        /// <summary>The pg m_ protoco l_ type.</summary>
        public static readonly ProtocolType PGM_PROTOCOL_TYPE = (ProtocolType)113;

        /// <summary>The log.</summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmSocket));

        #endregion

        #region Fields

        /// <summary>The _socket options.</summary>
        private IDictionary<int, uint> _socketOptions = new Dictionary<int, uint>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PgmSocket" /> class.
        /// </summary>
        public PgmSocket()
            : base(AddressFamily.InterNetwork, SocketType.Rdm, PGM_PROTOCOL_TYPE)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Sets the socket options.</summary>
        public IDictionary<int, uint> SocketOptions
        {
            set
            {
                _socketOptions = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The convert struct to bytes.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] ConvertStructToBytes(object obj)
        {
            var structSize = Marshal.SizeOf(obj);
            var allData = new byte[structSize];
            var handle = GCHandle.Alloc(allData, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return allData;
        }

        /// <summary>The enable gigabit.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool EnableGigabit(Socket socket)
        {
            return SetSocketOption(socket, "Gigabit", 1014, 1);
        }

        /// <summary>The set pgm option.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        public static void SetPgmOption(Socket socket, int option, byte[] value)
        {
            socket.SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
        }

        /// <summary>The set socket option.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="name">The name.</param>
        /// <param name="option">The option.</param>
        /// <param name="val">The val.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool SetSocketOption(Socket socket, string name, int option, uint val)
        {
            try
            {
                var bits = BitConverter.GetBytes(val);
                SetPgmOption(socket, option, bits);
                log.Info("Set: " + name + " Option : " + option + " value: " + val);
                return true;
            }
            catch (Exception failed)
            {
                log.Debug(name + " Option : " + option + " value: " + val, failed);
                return false;
            }
        }

        /// <summary>The add socket option.</summary>
        /// <param name="opt">The opt.</param>
        /// <param name="val">The val.</param>
        public void AddSocketOption(int opt, uint val)
        {
            _socketOptions[opt] = val;
        }

        /// <summary>The get receiver stats.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns>The <see cref="_RM_RECEIVER_STATS"/>.</returns>
        public unsafe _RM_RECEIVER_STATS GetReceiverStats(Socket socket)
        {
            var size = sizeof(_RM_RECEIVER_STATS);
            var data = socket.GetSocketOption(PGM_LEVEL, (SocketOptionName)1013, size);
            fixed (byte* pBytes = &data[0])
            {
                return *((_RM_RECEIVER_STATS*)pBytes);
            }
        }

        /// <summary>The set pgm option.</summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        public void SetPgmOption(int option, byte[] value)
        {
            try
            {
                SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
            }
            catch (Exception failed)
            {
                log.Warn("failed", failed);
            }
        }

        #endregion

        #region Methods

        /// <summary>The apply socket options.</summary>
        internal void ApplySocketOptions()
        {
            foreach (var option in _socketOptions.Keys)
            {
                SetSocketOption(this, option.ToString(), option, _socketOptions[option]);
            }
        }

        #endregion
    }
}