//   ===================================================================================
//   <copyright file="SubscriptionService.cs" company="TechieNotes">
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
//      The SubscriptionService.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Concurrent;

using TradeFx.Common.Interfaces;

namespace TradeFx.Common
{
    public abstract class SubscriptionService<TServiceRequest, TServiceResponse, TKey, TState> : IDisposable,
                                                                                                 ISubscriptionService
                                                                                                     <TServiceRequest,
                                                                                                     TServiceResponse,
                                                                                                     TKey, TState>
        where TServiceRequest : IServiceRequest
        where TServiceResponse : IServiceResponse
        where TKey : IKey
        where TState : new()
    {
        #region Public Methods and Operators

        public IServiceContext<TState> CalculationInitialized()
        {
            var context = new ServiceContext(new TState());
            CalculationInitialized(context, context.State);

            return context;
        }

        public abstract void CalculationInitialized(IServiceContext<TState> context, TState state);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TServiceRequest GetServiceRequest(TKey key)
        {
            throw new NotImplementedException();
        }

        public abstract bool TryCalculate(
            IServiceContext<TState> context,
            TKey key,
            TState state,
            out TServiceResponse response);

        public void UnRequest(TKey key)
        {
            throw new NotImplementedException();
        }

        #endregion

        public class ServiceContext : IServiceContext<TState>
        {
            #region Fields

            private readonly ConcurrentDictionary<IKey, IServiceResponse> _responses;

            private ConcurrentDictionary<IKey, IServiceRequest> _requests;

            #endregion

            #region Constructors and Destructors

            public ServiceContext(TState state)
            {
                State = state;
                _responses = new ConcurrentDictionary<IKey, IServiceResponse>();
            }

            #endregion

            #region Public Properties

            public TState State { get; }

            #endregion

            #region Public Methods and Operators

            public void AddRequest(IKey key, IServiceRequest request)
            {
                _requests[key] = request;
            }

            public bool TryGetResponse(IKey key, out IServiceResponse response)
            {
                return _responses.TryGetValue(key, out response);
            }

            #endregion
        }
    }
}