//   ===================================================================================
//   <copyright file="PriorityQueue.cs" company="TechieNotes">
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
//   <date>19-05-2015</date>
//   <summary>
//      The PriorityQueue.cs file.
//   </summary>
//   ===================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace TechieSocket.Net.Common
{
    public class PriorityQueue<TPriority, TItem>
    {
        #region Fields

        private readonly SortedDictionary<TPriority, Queue<TItem>> _subqueues;

        #endregion

        #region Constructors and Destructors

        public PriorityQueue(IComparer<TPriority> priorityComparer)
        {
            _subqueues = new SortedDictionary<TPriority, Queue<TItem>>(priorityComparer);
        }

        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                return _subqueues.Sum(q => q.Value.Count);
            }
        }

        public bool HasItems
        {
            get
            {
                return _subqueues.Any();
            }
        }

        #endregion

        #region Public Methods and Operators

        public TItem Dequeue()
        {
            if (_subqueues.Any())
            {
                return DequeueFromHighPriorityQueue();
            }
            throw new InvalidOperationException("The queue is empty");
        }

        public void Enqueue(TPriority priority, TItem item)
        {
            if (!_subqueues.ContainsKey(priority))
            {
                AddQueueOfPriority(priority);
            }

            _subqueues[priority].Enqueue(item);
        }

        public TItem Peek()
        {
            if (HasItems)
            {
                return _subqueues.First().Value.Peek();
            }
            throw new InvalidOperationException("The queue is empty");
        }

        #endregion

        #region Methods

        private void AddQueueOfPriority(TPriority priority)
        {
            _subqueues.Add(priority, new Queue<TItem>());
        }

        private TItem DequeueFromHighPriorityQueue()
        {
            var first = _subqueues.First();
            var nextItem = first.Value.Dequeue();
            if (!first.Value.Any())
            {
                _subqueues.Remove(first.Key);
            }
            return nextItem;
        }

        #endregion
    }
}