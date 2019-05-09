//  ===================================================================================
//  <copyright file="PgmConstants.cs" company="TechieNotes">
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
//     The PgmConstants.cs file.
//  </summary>
//  ===================================================================================
namespace Emcaster.Sockets
{
    /// <summary>The pgm constants.</summary>
    public class PgmConstants
    {
        #region Static Fields

        /// <summary>The ipprot o_ rm.</summary>
        public static readonly int IPPROTO_RM = 113;

        /// <summary>The ma x_ mcas t_ ttl.</summary>
        public static readonly int MAX_MCAST_TTL = 255;

        /// <summary>The r m_ ad d_ receiv e_ if.</summary>
        public static readonly int RM_ADD_RECEIVE_IF = RM_OPTIONSBASE + 8;

        // delete IP multicast incoming interface
        /// <summary>The r m_ de l_ receiv e_ if.</summary>
        public static readonly int RM_DEL_RECEIVE_IF = RM_OPTIONSBASE + 9;

        /// <summary>The r m_ flushcache.</summary>
        public static readonly int RM_FLUSHCACHE = RM_OPTIONSBASE + 3;

        /// <summary>The r m_ latejoin.</summary>
        public static readonly int RM_LATEJOIN = RM_OPTIONSBASE + 6;

        // options for setsockopt, getsockopt
        /// <summary>The r m_ optionsbase.</summary>
        public static readonly int RM_OPTIONSBASE = 1000;

        // Set/Query rate (Kb/Sec) + window size (Kb and/or MSec) -- described by RM_SEND_WINDOW below
        /// <summary>The r m_ rat e_ windo w_ size.</summary>
        public static readonly int RM_RATE_WINDOW_SIZE = RM_OPTIONSBASE + 1;

        /// <summary>The r m_ receive r_ statistics.</summary>
        public static readonly int RM_RECEIVER_STATISTICS = RM_OPTIONSBASE + 13;

        // Set the size of the next message -- (ULONG)

        // get sender statistics
        /// <summary>The r m_ sende r_ statistics.</summary>
        public static readonly int RM_SENDER_STATISTICS = RM_OPTIONSBASE + 5;

        /// <summary>The r m_ sende r_ windo w_ advanc e_ method.</summary>
        public static readonly int RM_SENDER_WINDOW_ADVANCE_METHOD = RM_OPTIONSBASE + 4;

        // allow a late-joiner to NAK any packet upto the lowest sequence Id

        // Set/Query the Window's Advance rate (has to be less that MAX_WINDOW_INCREMENT_PERCENTAGE)
        /// <summary>The r m_ sen d_ windo w_ ad v_ rate.</summary>
        public static readonly int RM_SEND_WINDOW_ADV_RATE = RM_OPTIONSBASE + 10;

        // Instruct to use parity-based forward error correction schemes

        // Set the Ttl of the MCast packets -- (ULONG)
        /// <summary>The r m_ se t_ mcas t_ ttl.</summary>
        public static readonly int RM_SET_MCAST_TTL = RM_OPTIONSBASE + 12;

        /// <summary>The r m_ se t_ messag e_ boundary.</summary>
        public static readonly int RM_SET_MESSAGE_BOUNDARY = RM_OPTIONSBASE + 2;

        /// <summary>The r m_ se t_ sen d_ if.</summary>
        public static readonly int RM_SET_SEND_IF = RM_OPTIONSBASE + 7;

        /// <summary>The r m_ us e_ fec.</summary>
        public static readonly int RM_USE_FEC = RM_OPTIONSBASE + 11;

        #endregion

        // get receiver statistics
    }

    /// <summary>The e windo w_ advanc e_ method.</summary>
    internal enum eWINDOW_ADVANCE_METHOD
    {
        /// <summary>The e_ windo w_ advanc e_ b y_ time.</summary>
        E_WINDOW_ADVANCE_BY_TIME = 1, // Default mode
        /// <summary>The e_ windo w_ us e_ a s_ dat a_ cache.</summary>
        E_WINDOW_USE_AS_DATA_CACHE
    };

    // ==============================================================
    // Structures
    /// <summary>The _ r m_ sen d_ window.</summary>
    public struct _RM_SEND_WINDOW
    {
        #region Fields

        /// <summary>The rate kbits per sec.</summary>
        public uint RateKbitsPerSec; // Send rate

        /// <summary>The window size in bytes.</summary>
        public uint WindowSizeInBytes;

        /// <summary>The window size in m secs.</summary>
        public uint WindowSizeInMSecs;

        #endregion
    }

    /// <summary>The _ r m_ sende r_ stats.</summary>
    public struct _RM_SENDER_STATS
    {
        #region Fields

        /// <summary>The buffer space available.</summary>
        public ulong BufferSpaceAvailable; // # partial messages dropped

        /// <summary>The data bytes sent.</summary>
        public ulong DataBytesSent; // # client data bytes sent out so far

        /// <summary>The leading edge seq id.</summary>
        public ulong LeadingEdgeSeqId; // largest (newest) Sequence Id in the window

        /// <summary>The naks received.</summary>
        public ulong NaksReceived; // # NAKs received so far

        /// <summary>The naks received too late.</summary>
        public ulong NaksReceivedTooLate; // # NAKs recvd after window advanced

        /// <summary>The num naks after r data.</summary>
        public ulong NumNaksAfterRData; // # NAKs yet to be responded to

        /// <summary>The num outstanding naks.</summary>
        public ulong NumOutstandingNaks; // # NAKs yet to be responded to

        /// <summary>The rate k bits per sec last.</summary>
        public ulong RateKBitsPerSecLast; // Send-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY

        /// <summary>The rate k bits per sec overall.</summary>
        public ulong RateKBitsPerSecOverall; // Internally calculated send-rate from the beginning

        /// <summary>The repair packets sent.</summary>
        public ulong RepairPacketsSent; // # Repairs (RDATA) sent so far

        /// <summary>The total bytes sent.</summary>
        public ulong TotalBytesSent; // SPM, OData and RData bytes

        /// <summary>The trailing edge seq id.</summary>
        public ulong TrailingEdgeSeqId; // smallest (oldest) Sequence Id in the window

        #endregion
    }

    /// <summary>The _ r m_ receive r_ stats.</summary>
    public struct _RM_RECEIVER_STATS
    {
        #region Fields

        /// <summary>The average sequences in window.</summary>
        public ulong AverageSequencesInWindow;

        /// <summary>The data bytes received.</summary>
        public ulong DataBytesReceived; // # client data bytes received out so far

        /// <summary>The first nak sequence number.</summary>
        public ulong FirstNakSequenceNumber; // # First Outstanding Nak

        /// <summary>The leading edge seq id.</summary>
        public ulong LeadingEdgeSeqId; // largest (newest) Sequence Id in the window

        /// <summary>The max sequences in window.</summary>
        public ulong MaxSequencesInWindow;

        /// <summary>The min sequences in window.</summary>
        public ulong MinSequencesInWindow;

        /// <summary>The num data packets buffered.</summary>
        public ulong NumDataPacketsBuffered; // # Data packets currently buffered by transport

        /// <summary>The num duplicate data packets.</summary>
        public ulong NumDuplicateDataPackets; // # RData sequences received

        /// <summary>The num o data packets received.</summary>
        public ulong NumODataPacketsReceived; // # OData sequences received

        /// <summary>The num outstanding naks.</summary>
        public ulong NumOutstandingNaks; // # Sequences for which Ncfs have been received, but no data

        /// <summary>The num pending naks.</summary>
        public ulong NumPendingNaks; // # Sequences waiting for Ncfs

        /// <summary>The num r data packets received.</summary>
        public ulong NumRDataPacketsReceived; // # RData sequences received

        /// <summary>The rate k bits per sec last.</summary>
        public ulong RateKBitsPerSecLast; // Receive-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY

        /// <summary>The rate k bits per sec overall.</summary>
        public ulong RateKBitsPerSecOverall; // Internally calculated Receive-rate from the beginning

        /// <summary>The total bytes received.</summary>
        public ulong TotalBytesReceived; // SPM, OData and RData bytes

        /// <summary>The total parity naks sent.</summary>
        public ulong TotalParityNaksSent; // # Parity NAKs sent so far

        /// <summary>The total selective naks sent.</summary>
        public ulong TotalSelectiveNaksSent; // # Selective NAKs sent so far

        /// <summary>The trailing edge seq id.</summary>
        public ulong TrailingEdgeSeqId; // smallest (oldest) Sequence Id in the window

        #endregion
    }
}