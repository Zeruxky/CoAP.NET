namespace WorldDirect.CoAP.Stack
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a chain of filters.
    /// </summary>
    /// <typeparam name="TFilter">the type of filters</typeparam>
    /// <typeparam name="TNextFilter">the type of next filters</typeparam>
    public interface IChain<TFilter, TNextFilter>
    {
        /// <summary>
        /// Gets the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/> with the specified <paramref name="name"/> in this chain.
        /// </summary>
        /// <param name="name">the filter's name we are looking for</param>
        /// <returns>the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/> with the given name, or null if not found</returns>
        IEntry<TFilter, TNextFilter> GetEntry(String name);
        /// <summary>
        /// Gets the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/> with the specified <paramref name="filter"/> in this chain.
        /// </summary>
        /// <param name="filter">the filter we are looking for</param>
        /// <returns>the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/>, or null if not found</returns>
        IEntry<TFilter, TNextFilter> GetEntry(TFilter filter);
        /// <summary>
        /// Gets the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/> with the specified <paramref name="filterType"/> in this chain.
        /// </summary>
        /// <remarks>If there's more than one filter with the specified type, the first match will be chosen.</remarks>
        /// <param name="filterType">the type of filter we are looking for</param>
        /// <returns>the <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/>, or null if not found</returns>
        IEntry<TFilter, TNextFilter> GetEntry(Type filterType);
        /// <summary>
        /// Gets the <typeparamref name="TFilter"/> with the specified <paramref name="name"/> in this chain.
        /// </summary>
        /// <param name="name">the filter's name</param>
        /// <returns>the <typeparamref name="TFilter"/>, or null if not found</returns>
        TFilter Get(String name);
        /// <summary>
        /// Gets the <typeparamref name="TNextFilter"/> of the <typeparamref name="TFilter"/>
        /// with the specified <paramref name="name"/> in this chain.
        /// </summary>
        /// <param name="name">the filter's name</param>
        /// <returns>the <typeparamref name="TNextFilter"/>, or null if not found</returns>
        TNextFilter GetNextFilter(String name);
        /// <summary>
        /// Gets the <typeparamref name="TNextFilter"/> of the <typeparamref name="TFilter"/>
        /// with the specified <paramref name="filter"/> in this chain.
        /// </summary>
        /// <param name="filter">the filter</param>
        /// <returns>the <typeparamref name="TNextFilter"/>, or null if not found</returns>
        TNextFilter GetNextFilter(TFilter filter);
        /// <summary>
        /// Gets the <typeparamref name="TNextFilter"/> of the <typeparamref name="TFilter"/>
        /// with the specified <paramref name="filterType"/> in this chain.
        /// </summary>
        /// <remarks>If there's more than one filter with the specified type, the first match will be chosen.</remarks>
        /// <param name="filterType">the type of filter</param>
        /// <returns>the <typeparamref name="TNextFilter"/>, or null if not found</returns>
        TNextFilter GetNextFilter(Type filterType);
        /// <summary>
        /// Gets all <see cref="IEntry&lt;TFilter, TNextFilter&gt;"/>s in this chain.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntry<TFilter, TNextFilter>> GetAll();
        /// <summary>
        /// Checks if this chain contains a filter with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">the filter's name</param>
        /// <returns>true if this chain contains a filter with the specified <paramref name="name"/></returns>
        Boolean Contains(String name);
        /// <summary>
        /// Checks if this chain contains the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">the filter</param>
        /// <returns>true if this chain contains the specified <paramref name="filter"/></returns>
        Boolean Contains(TFilter filter);
        /// <summary>
        /// Checks if this chain contains a filter with the specified <paramref name="filterType"/>.
        /// </summary>
        /// <param name="filterType">the filter's type</param>
        /// <returns>true if this chain contains a filter with the specified <paramref name="filterType"/></returns>
        Boolean Contains(Type filterType);
        /// <summary>
        /// Adds the specified filter with the specified name at the beginning of this chain.
        /// </summary>
        /// <param name="name">the filter's name</param>
        /// <param name="filter">the filter to add</param>
        void AddFirst(String name, TFilter filter);
        /// <summary>
        /// Adds the specified filter with the specified name at the end of this chain.
        /// </summary>
        /// <param name="name">the filter's name</param>
        /// <param name="filter">the filter to add</param>
        void AddLast(String name, TFilter filter);
        /// <summary>
        /// Adds the specified filter with the specified name just before the filter whose name is
        /// <paramref name="baseName"/> in this chain.
        /// </summary>
        /// <param name="baseName">the targeted filter's name</param>
        /// <param name="name">the filter's name</param>
        /// <param name="filter">the filter to add</param>
        void AddBefore(String baseName, String name, TFilter filter);
        /// <summary>
        /// Adds the specified filter with the specified name just after the filter whose name is
        /// <paramref name="baseName"/> in this chain.
        /// </summary>
        /// <param name="baseName">the targeted filter's name</param>
        /// <param name="name">the filter's name</param>
        /// <param name="filter">the filter to add</param>
        void AddAfter(String baseName, String name, TFilter filter);
        /// <summary>
        /// Replace the filter with the specified name with the specified new filter.
        /// </summary>
        /// <param name="name">the name of the filter to replace</param>
        /// <param name="newFilter">the new filter</param>
        /// <returns>the old filter</returns>
        TFilter Replace(String name, TFilter newFilter);
        /// <summary>
        /// Replace the specified filter with the specified new filter.
        /// </summary>
        /// <param name="oldFilter">the filter to replace</param>
        /// <param name="newFilter">the new filter</param>
        void Replace(TFilter oldFilter, TFilter newFilter);
        /// <summary>
        /// Removes the filter with the specified name from this chain.
        /// </summary>
        /// <param name="name">the name of the filter to remove</param>
        /// <returns>the removed filter</returns>
        TFilter Remove(String name);
        /// <summary>
        /// Removes the specified filter.
        /// </summary>
        /// <param name="filter">the filter to remove</param>
        void Remove(TFilter filter);
        /// <summary>
        /// Removes all filters added to this chain.
        /// </summary>
        void Clear();
    }
}