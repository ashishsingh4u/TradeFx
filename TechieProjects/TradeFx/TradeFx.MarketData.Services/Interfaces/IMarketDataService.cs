//   ===================================================================================
//   <copyright file="IMarketDataService.cs" company="TechieNotes">
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
//      The IMarketDataService.cs file.
//   </summary>
//   ===================================================================================

namespace TradeFx.MarketData.Services.Interfaces
{
    /// <summary>The MarketData interface.</summary>
    public interface IMarketDataService
    {
        #region Public Methods and Operators

        /// <summary>The get market data.</summary>
        /// <returns>The <see cref="string" />.</returns>
        string GetMarketData();

        /// <summary>The register currency pair.</summary>
        void RegisterCurrencyPair();

        #endregion
    }
}