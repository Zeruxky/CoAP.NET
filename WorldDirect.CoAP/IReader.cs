// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP
{
    using System;

    /// <summary>
    /// Defines functionality to read a <typeparamref name="TResult"/> from a specified <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IReader<TResult>
    {
        /// <summary>
        /// Reads the <typeparamref name="TResult"/> from the specified <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}"/> that contains the <typeparamref name="TResult"/>.</param>
        /// <param name="result">The <typeparamref name="TResult"/> that was read from the <see cref="ReadOnlyMemory{T}"/>.</param>
        /// <returns>The read number of <see langword="byte"/>s.</returns>
        int Read(ReadOnlyMemory<byte> value, out TResult result);
    }
}
