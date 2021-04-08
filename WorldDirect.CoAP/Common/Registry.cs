// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Provides common functionality to handle, receive and store <typeparamref name="T"/>s in a registry.
    /// </summary>
    /// <typeparam name="T">The elements that are stored in the registry.</typeparam>
    public abstract class Registry<T> : ICollection<T>
    {
        private readonly List<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry{T}"/> class.
        /// </summary>
        /// <param name="items">The <typeparamref name="T"/>s that should represents the registry's items.</param>
        protected Registry(IEnumerable<T> items)
        {
            this.items = items.ToList();
        }

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (!this.Contains(item))
            {
                this.items.Add(item);
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this.items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
