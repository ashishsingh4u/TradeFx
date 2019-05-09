//   ===================================================================================
//   <copyright file="BlockingQueue.cs" company="TechieNotes">
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
//   <date>22-05-2015</date>
//   <summary>
//      The BlockingQueue.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace TechieSocket.Net.Common
{
    public class BlockingQueue<T> : IDisposable
    {
        #region Fields

        private readonly BlockingCollection<T> _collection;

        private readonly IObservable<T> _observable;

        private readonly List<IDisposable> _subscriptions;

        #endregion

        #region Constructors and Destructors

        public BlockingQueue()
        {
            _collection = new BlockingCollection<T>(new ConcurrentQueue<T>());
            _observable = _collection.GetConsumingEnumerable().ToObservable(TaskPoolScheduler.Default);
            _subscriptions = new List<IDisposable>();
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _subscriptions.ForEach(subscription => subscription.Dispose());
            _subscriptions.Clear();
            _collection.Dispose();
        }

        public void Publish(T item)
        {
            _collection.Add(item);
        }

        public void Subscribe(Action<T> handler, Action<Exception> errorhandler)
        {
            _subscriptions.Add(_observable.Subscribe(handler, errorhandler));
        }

        #endregion
    }
}