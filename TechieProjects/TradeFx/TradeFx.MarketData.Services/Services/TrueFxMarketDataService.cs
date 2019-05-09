//   ===================================================================================
//   <copyright file="TrueFxMarketDataService.cs" company="TechieNotes">
//   ===================================================================================
//    TechieNotes Utilities & Best Practices
//    Samples and Guidelines for Winform & ASP.net development
//   ===================================================================================
//    Copyright (c) TechieNotes.  All rights reserved.
//    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//    OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//    LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//    FITNESS FOR A PARTICULAR PURPOSE.
//   ===================================================================================
//    The example companies, organizations, products, domain names,
//    e-mail addresses, logos, people, places, and events depicted
//    herein are fictitious.  No association with any real company,
//    organization, product, domain name, email address, logo, person,
//    places, or events is intended or should be inferred.
//   ===================================================================================
//   </copyright>
//   <author>Ashish Singh</author>
//   <email>mailto:ashishsingh4u@gmail.com</email>
//   <date>22-06-2015</date>
//   <summary>
//      The TrueFxMarketDataService.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.IO;
using System.Linq;
using System.Net;

using TradeFx.MarketData.Services.Contracts;
using TradeFx.MarketData.Services.Interfaces;

namespace TradeFx.MarketData.Services.Services
{
    /// <summary>The TrueFx market data service.</summary>
    public class TrueFxMarketDataService : MarketDataService, ITrueFxMarketDataService
    {
        #region Constants

        /// <summary>The authentication url.</summary>
        private const string AuthenticationUrl =
            "http://webrates.truefx.com/rates/connect.html?u={0}&p={1}&q=ozrates&c={2}&f=csv&s=n";

        /// <summary>The subscribe feed url.</summary>
        private const string SubscribeFeedUrl = "http://webrates.truefx.com/rates/connect.html?id={0}";

        #endregion

        #region Fields

        /// <summary>The _subscribed feeds.</summary>
        private string _subscribedFeeds;

        #endregion

        #region Public Methods and Operators

        /// <summary>The get prices.</summary>
        /// <param name="token">The token.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void GetPrices(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("token cannot be empty.");
            }

            var feed = GetFeed(string.Format(SubscribeFeedUrl, token));
            if (feed.EndsWith("\n\r\n"))
            {
                feed = feed.Replace("\n\r\n", string.Empty);
            }

            var feeds = feed.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var currencies = feeds.Select(Price.FromFeed).ToList();
        }

        /// <summary>The initialize.</summary>
        public void Initialize()
        {
            _subscribedFeeds =
                "EUR/USD,GBP/USD,EUR/GBP,AUD/USD,USD/JPY,AUD/USD,USD/CHF,USD/CAD,USD/HKD,USD/SEK,USD/NZD,USD/KRW,USD/SGD,USD/NOK,USD/MXN,USD/INR";
            GetPrices(GetFeedToken("ashishsingh", "dragon1982"));
        }

        #endregion

        #region Methods

        /// <summary>The get feed.</summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="string" />.</returns>
        private static string GetFeed(string url)
        {
            string data = null;
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(url))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>The get feed token.</summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="string" />.</returns>
        private string GetFeedToken(string username, string password)
        {
            return GetFeed(string.Format(AuthenticationUrl, username, password, _subscribedFeeds));
        }

        #endregion
    }
}