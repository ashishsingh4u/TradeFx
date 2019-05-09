//   ===================================================================================
//   <copyright file="IServiceContext.cs" company="TechieNotes">
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
//   <date>28-06-2015</date>
//   <summary>
//      The IServiceContext.cs file.
//   </summary>
//   ===================================================================================

namespace TradeFx.Common.Interfaces
{
    public interface IServiceContext<TState>
    {
        #region Public Properties

        TState State { get; }

        #endregion

        #region Public Methods and Operators

        void AddRequest(IKey key, IServiceRequest request);

        bool TryGetResponse(IKey key, out IServiceResponse response);

        #endregion
    }
}