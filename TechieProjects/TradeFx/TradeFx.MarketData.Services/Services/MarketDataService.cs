//   ===================================================================================
//   <copyright file="MarketDataService.cs" company="TechieNotes">
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
//      The MarketDataService.cs file.
//   </summary>
//   ===================================================================================

using System;

using TradeFx.MarketData.Services.Interfaces;

namespace TradeFx.MarketData.Services.Services
{
    /// <summary>The market data service.</summary>
    public class MarketDataService : IMarketDataService
    {
        #region Public Methods and Operators

        /// <summary>Initializes a new instance of the <see cref="MarketDataService" /> class.</summary>
        /// <summary>The get market data.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string GetMarketData()
        {
            return string.Empty;
        }

        /// <summary>The register currency pair.</summary>
        public virtual void RegisterCurrencyPair()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}