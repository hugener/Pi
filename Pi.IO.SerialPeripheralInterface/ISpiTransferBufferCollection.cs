// <copyright file="ISpiTransferBufferCollection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// A collection of transfer buffers that can be used to read from / write to the SPI bus.
    /// </summary>
    public interface ISpiTransferBufferCollection : IDisposable, IEnumerable<ISpiTransferBuffer>
    {
        /// <summary>
        /// Gets the number of <see cref="ISpiTransferBuffer"/> structures.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Can be used to request a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        ISpiTransferBuffer this[int index] { get; }

        /// <summary>
        /// A method that returns a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        ISpiTransferBuffer Get(int index);
    }
}