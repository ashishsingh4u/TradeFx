//  ===================================================================================
//  <copyright file="Price.cs" company="TechieNotes">
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
//  <date>01-01-2013</date>
//  <summary>
//     The Price.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics;

namespace TradeFx.MarketData.Services.Contracts
{
    /// <summary>The currency.</summary>
    [DebuggerDisplay("{CurrencyPair}")]
    public struct Price
    {
        #region Public Properties

        /// <summary>Gets the bid point.</summary>
        public string BidPoint { get; private set; }

        /// <summary>Gets the bid rate.</summary>
        public decimal BidRate { get; private set; }

        /// <summary>Gets the big bid.</summary>
        public decimal BigBid { get; private set; }

        /// <summary>Gets the big offer.</summary>
        public decimal BigOffer { get; private set; }

        /// <summary>Gets the currency pair.</summary>
        public string CurrencyPair { get; private set; }

        /// <summary>Gets the high offer rate.</summary>
        public decimal HighOfferRate { get; private set; }

        /// <summary>Gets the low bid rate.</summary>
        public decimal LowBidRate { get; private set; }

        /// <summary>Gets the offer point.</summary>
        public string OfferPoint { get; private set; }

        /// <summary>Gets the offer rate.</summary>
        public decimal OfferRate { get; private set; }

        /// <summary>Gets the open rate.</summary>
        public decimal OpenRate { get; private set; }

        /// <summary>Gets the raw mid.</summary>
        public decimal RawMid { get; private set; }

        /// <summary>Gets the time stamp.</summary>
        public string TimeStamp { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Format : EUR/USD,1329746416521,1.32,610,1.32,615,1.31735,1.32771,1.31754
        ///     Price-pair symbol (AUD/USD)
        ///     Millisecond timestamp (1253890249578)
        ///     Bid big figure (for example, 0.86)
        ///     Bid points (for example, 565)
        ///     Offer big figure (for example, 0.86)
        ///     Offer points (for example, 583)
        ///     High, the greatest offer price since the currency pair’s roll time (for example, 0.86148)
        ///     Low, the smallest bid price since the currency pair’s roll time (for example, 0.87078)
        ///     Open, the mid price at the currency pair’s roll time (for example, 0.86821)</summary>
        /// <param name="currencyfeed">Price feed.</param>
        /// <returns>The <see cref="Price"/>.</returns>
        public static Price FromFeed(string currencyfeed)
        {
            if (string.IsNullOrEmpty(currencyfeed))
            {
                throw new ArgumentException("Price feed cannot be empty.");
            }

            var values = currencyfeed.Split(',');
            if (values == null || values.Length != 9)
            {
                throw new ArgumentException("Invalid feed");
            }

            var currency = new Price
                               {
                                   CurrencyPair = values[0], 
                                   TimeStamp = values[1], 
                                   BigBid = decimal.Parse(values[2]), 
                                   BidPoint = values[3], 
                                   BigOffer = decimal.Parse(values[4]), 
                                   OfferPoint = values[5], 
                                   HighOfferRate = decimal.Parse(values[6]), 
                                   LowBidRate = decimal.Parse(values[7]), 
                                   OpenRate = decimal.Parse(values[8]), 
                                   BidRate = decimal.Parse(string.Concat(values[2], values[3])), 
                                   OfferRate = decimal.Parse(string.Concat(values[4], values[5])) 
                               };
            currency.RawMid = (currency.BidRate + currency.OfferRate) / 2;
            return currency;
        }

        #endregion
    }
}