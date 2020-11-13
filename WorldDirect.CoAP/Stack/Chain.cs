/*
 * Copyright (c) 2011-2014, Longxiang He <helongxiang@smeshlink.com>,
 * SmeshLink Technology Co.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY.
 * 
 * This file is part of the CoAP.NET, a CoAP framework in C#.
 * Please see README for more information.
 */

namespace WorldDirect.CoAP.Stack
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Abstract implementation of <see cref="IChain&lt;TFilter, TNextFilter&gt;"/>
    /// </summary>
    /// <typeparam name="TChain">the actual type of the chain</typeparam>
    /// <typeparam name="TFilter">the type of filters</typeparam>
    /// <typeparam name="TNextFilter">the type of next filters</typeparam>
    public abstract class Chain<TChain, TFilter, TNextFilter> : IChain<TFilter, TNextFilter>
        where TChain : Chain<TChain, TFilter, TNextFilter>
    {
        private readonly IDictionary<String, Entry> _name2entry = new Dictionary<String, Entry>();
        private readonly Entry _head;
        private readonly Entry _tail;
        private readonly Func<TFilter, TFilter, Boolean> _equalsFunc;
        private readonly Func<TChain, Entry, Entry, String, TFilter, Entry> _entryFactory;

        /// <summary>
        /// Instantiates.
        /// </summary>
        /// <param name="nextFilterFactory">the factory to create <typeparamref name="TNextFilter"/>s by (entry)</param>
        /// <param name="headFilterFactory">the factory to create the head <typeparamref name="TFilter"/></param>
        /// <param name="tailFilterFactory">the factory to create the tail <typeparamref name="TFilter"/></param>
        protected Chain(Func<Entry, TNextFilter> nextFilterFactory, Func<TFilter> headFilterFactory, Func<TFilter> tailFilterFactory)
            : this((chain, prev, next, name, filter) => new Entry(chain, prev, next, name, filter, nextFilterFactory),
            headFilterFactory, tailFilterFactory)
        { }

        /// <summary>
        /// Instantiates.
        /// </summary>
        /// <param name="entryFactory">the factory to create entries by (chain, prev, next, name, filter)</param>
        /// <param name="headFilterFactory">the factory to create the head <typeparamref name="TFilter"/></param>
        /// <param name="tailFilterFactory">the factory to create the tail <typeparamref name="TFilter"/></param>
        protected Chain(Func<TChain, Entry, Entry, String, TFilter, Entry> entryFactory,
            Func<TFilter> headFilterFactory, Func<TFilter> tailFilterFactory)
            : this(entryFactory, headFilterFactory, tailFilterFactory, (t1, t2) => Object.ReferenceEquals(t1, t2))
        { }

        /// <summary>
        /// Instantiates.
        /// </summary>
        /// <param name="entryFactory">the factory to create entries by (chain, prev, next, name, filter)</param>
        /// <param name="headFilterFactory">the factory to create the head <typeparamref name="TFilter"/></param>
        /// <param name="tailFilterFactory">the factory to create the tail <typeparamref name="TFilter"/></param>
        /// <param name="equalsFunc">the function to check equality between two <typeparamref name="TFilter"/>s</param>
        protected Chain(Func<TChain, Entry, Entry, String, TFilter, Entry> entryFactory, 
            Func<TFilter> headFilterFactory, Func<TFilter> tailFilterFactory,
            Func<TFilter, TFilter, Boolean> equalsFunc)
        {
            _equalsFunc = equalsFunc;
            _entryFactory = entryFactory;
            _head = entryFactory((TChain)this, null, null, "head", headFilterFactory());
            _tail = entryFactory((TChain)this, _head, null, "tail", tailFilterFactory());
            _head._nextEntry = _tail;
        }

        /// <summary>
        /// Head of this chain.
        /// </summary>
        protected Entry Head
        {
            get { return _head; }
        }

        /// <summary>
        /// Tail of this chain.
        /// </summary>
        protected Entry Tail
        {
            get { return _tail; }
        }

        /// <inheritdoc/>
        public IEntry<TFilter, TNextFilter> GetEntry(String name)
        {
            Entry e;
            _name2entry.TryGetValue(name, out e);
            return e;
        }

        /// <inheritdoc/>
        public TFilter Get(String name)
        {
            IEntry<TFilter, TNextFilter> e = GetEntry(name);
            return e == null ? default(TFilter) : e.Filter;
        }

        /// <inheritdoc/>
        public IEntry<TFilter, TNextFilter> GetEntry(TFilter filter)
        {
            Entry e = _head._nextEntry;
            while (e != _tail)
            {
                if (_equalsFunc(e.Filter, filter))
                    return e;
                e = e._nextEntry;
            }
            return null;
        }

        /// <inheritdoc/>
        public IEntry<TFilter, TNextFilter> GetEntry(Type filterType)
        {
            Entry e = _head._nextEntry;
            while (e != _tail)
            {
                if (filterType.IsAssignableFrom(e.Filter.GetType()))
                    return e;
                e = e._nextEntry;
            }
            return null;
        }

        /// <inheritdoc/>
        public TNextFilter GetNextFilter(String name)
        {
            IEntry<TFilter, TNextFilter> e = GetEntry(name);
            return e == null ? default(TNextFilter) : e.NextFilter;
        }

        /// <inheritdoc/>
        public TNextFilter GetNextFilter(TFilter filter)
        {
            IEntry<TFilter, TNextFilter> e = GetEntry(filter);
            return e == null ? default(TNextFilter) : e.NextFilter;
        }

        /// <inheritdoc/>
        public TNextFilter GetNextFilter(Type filterType)
        {
            IEntry<TFilter, TNextFilter> e = GetEntry(filterType);
            return e == null ? default(TNextFilter) : e.NextFilter;
        }

        /// <inheritdoc/>
        public IEnumerable<IEntry<TFilter, TNextFilter>> GetAll()
        {
            Entry e = _head._nextEntry;
            while (e != _tail)
            {
                yield return e;
                e = e._nextEntry;
            }
        }

        /// <inheritdoc/>
        public Boolean Contains(String name)
        {
            return GetEntry(name) != null;
        }

        /// <inheritdoc/>
        public Boolean Contains(TFilter filter)
        {
            return GetEntry(filter) != null;
        }

        /// <inheritdoc/>
        public Boolean Contains(Type filterType)
        {
            return GetEntry(filterType) != null;
        }

        /// <inheritdoc/>
        public void AddFirst(String name, TFilter filter)
        {
            CheckAddable(name);
            Register(_head, name, filter);
        }

        /// <inheritdoc/>
        public void AddLast(String name, TFilter filter)
        {
            CheckAddable(name);
            Register(_tail._prevEntry, name, filter);
        }

        /// <inheritdoc/>
        public void AddBefore(String baseName, String name, TFilter filter)
        {
            Entry baseEntry = CheckOldName(baseName);
            CheckAddable(name);
            Register(baseEntry._prevEntry, name, filter);
        }

        /// <inheritdoc/>
        public void AddAfter(String baseName, String name, TFilter filter)
        {
            Entry baseEntry = CheckOldName(baseName);
            CheckAddable(name);
            Register(baseEntry, name, filter);
        }

        /// <inheritdoc/>
        public TFilter Replace(String name, TFilter newFilter)
        {
            Entry entry = CheckOldName(name);
            TFilter oldFilter = entry.Filter;
            entry.Filter = newFilter;
            return oldFilter;
        }

        /// <inheritdoc/>
        public void Replace(TFilter oldFilter, TFilter newFilter)
        {
            Entry e = _head._nextEntry;
            while (e != _tail)
            {
                if (_equalsFunc(e.Filter, oldFilter))
                {
                    e.Filter = newFilter;
                    return;
                }
                e = e._nextEntry;
            }
            throw new ArgumentException("Filter not found: " + oldFilter.GetType().Name);
        }

        /// <inheritdoc/>
        public TFilter Remove(String name)
        {
            Entry entry = CheckOldName(name);
            Deregister(entry);
            return entry.Filter;
        }

        /// <inheritdoc/>
        public void Remove(TFilter filter)
        {
            Entry e = _head._nextEntry;
            while (e != _tail)
            {
                if (_equalsFunc(e.Filter, filter))
                {
                    Deregister(e);
                    return;
                }
                e = e._nextEntry;
            }
            throw new ArgumentException("Filter not found: " + filter.GetType().Name);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var entry in _name2entry.Values)
            {
                Deregister((Entry)entry);
            }
        }

        private void CheckAddable(String name)
        {
            if (_name2entry.ContainsKey(name))
                throw new ArgumentException("Other filter is using the same name '" + name + "'");
        }

        private Entry CheckOldName(String baseName)
        {
            return (Entry)_name2entry[baseName];
        }

        private void Register(Entry prevEntry, String name, TFilter filter)
        {
            Entry newEntry = _entryFactory((TChain)this, prevEntry, prevEntry._nextEntry, name, filter);

            OnPreAdd(newEntry);

            prevEntry._nextEntry._prevEntry = newEntry;
            prevEntry._nextEntry = newEntry;
            _name2entry.Add(name, newEntry);

            OnPostAdd(newEntry);
        }

        private void Deregister(Entry entry)
        {
            OnPreRemove(entry);
            Deregister0(entry);
            OnPostRemove(entry);
        }

        /// <summary>
        /// Deregister an entry from this chain.
        /// </summary>
        protected void Deregister0(Entry entry)
        {
            Entry prevEntry = entry._prevEntry;
            Entry nextEntry = entry._nextEntry;
            prevEntry._nextEntry = nextEntry;
            nextEntry._prevEntry = prevEntry;

            _name2entry.Remove(entry.Name);
        }

        /// <summary>
        /// Fires before the entry is added to this chain.
        /// </summary>
        protected virtual void OnPreAdd(Entry entry) { }

        /// <summary>
        /// Fires after the entry is added to this chain.
        /// </summary>
        protected virtual void OnPostAdd(Entry entry) { }

        /// <summary>
        /// Fires before the entry is removed to this chain.
        /// </summary>
        protected virtual void OnPreRemove(Entry entry) { }

        /// <summary>
        /// Fires after the entry is removed to this chain.
        /// </summary>
        protected virtual void OnPostRemove(Entry entry) { }

        /// <summary>
        /// Represents an entry of filter in the chain.
        /// </summary>
        public class Entry : IEntry<TFilter, TNextFilter>
        {
            private readonly TChain _chain;
            private readonly String _name;
            internal Entry _prevEntry;
            internal Entry _nextEntry;
            private TFilter _filter;
            private readonly TNextFilter _nextFilter;

            /// <summary>
            /// Instantiates.
            /// </summary>
            /// <param name="chain">the chain this entry belongs to</param>
            /// <param name="prevEntry">the previous one</param>
            /// <param name="nextEntry">the next one</param>
            /// <param name="name">the name of this entry</param>
            /// <param name="filter">the associated <typeparamref name="TFilter"/></param>
            /// <param name="nextFilterFactory">the factory to create <typeparamref name="TNextFilter"/> by (entry)</param>
            public Entry(TChain chain, Entry prevEntry, Entry nextEntry,
                String name, TFilter filter, Func<Entry, TNextFilter> nextFilterFactory)
            {
                if (filter == null)
                    throw new ArgumentNullException("filter");
                if (name == null)
                    throw new ArgumentNullException("name");

                _chain = chain;
                _prevEntry = prevEntry;
                _nextEntry = nextEntry;
                _name = name;
                _filter = filter;
                _nextFilter = nextFilterFactory(this);
            }

            /// <inheritdoc/>
            public String Name
            {
                get { return _name; }
            }

            /// <inheritdoc/>
            public TFilter Filter
            {
                get { return _filter; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    _filter = value;
                }
            }

            /// <inheritdoc/>
            public TNextFilter NextFilter
            {
                get { return _nextFilter; }
            }

            /// <summary>
            /// Gets the chain this entry belongs to.
            /// </summary>
            public TChain Chain
            {
                get { return _chain; }
            }

            /// <summary>
            /// Gets the previous entry in the chain.
            /// </summary>
            public Entry PrevEntry
            {
                get { return _prevEntry; }
            }

            /// <summary>
            /// Gets the next entry in the chain.
            /// </summary>
            public Entry NextEntry
            {
                get { return _nextEntry; }
            }

            /// <inheritdoc/>
            public void AddBefore(String name, TFilter filter)
            {
                _chain.AddBefore(Name, name, filter);
            }

            /// <inheritdoc/>
            public void AddAfter(String name, TFilter filter)
            {
                _chain.AddAfter(Name, name, filter);
            }

            /// <inheritdoc/>
            public void Replace(TFilter newFilter)
            {
                _chain.Replace(Name, newFilter);
            }

            /// <inheritdoc/>
            public void Remove()
            {
                _chain.Remove(Name);
            }

            /// <inheritdoc/>
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder();

                // Add the current filter
                sb.Append("('").Append(Name).Append('\'');

                // Add the previous filter
                sb.Append(", prev: '");

                if (_prevEntry != null)
                {
                    sb.Append(_prevEntry.Name);
                    sb.Append(':');
                    sb.Append(_prevEntry.Filter.GetType().Name);
                }
                else
                {
                    sb.Append("null");
                }

                // Add the next filter
                sb.Append("', next: '");

                if (_nextEntry != null)
                {
                    sb.Append(_nextEntry.Name);
                    sb.Append(':');
                    sb.Append(_nextEntry.Filter.GetType().Name);
                }
                else
                {
                    sb.Append("null");
                }

                sb.Append("')");
                return sb.ToString();
            }
        }
    }
}
