// <copyright file="IGpioConnectionDriverFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Interface for implementing a factory for <see cref="IGpioConnectionDriver" />.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public interface IGpioConnectionDriverFactory : IDisposable
    {
        /// <summary>
        /// Creates an <see cref="IGpioConnectionDriver"/>.
        /// </summary>
        /// <returns>A new <see cref="IGpioConnectionDriver"/>.</returns>
        IGpioConnectionDriver Get();

        /// <summary>
        /// Disposes the specified gpio connection driver if created by the factory.
        /// </summary>
        /// <param name="gpioConnectionDriver">The gpio connection driver.</param>
        void Dispose(IGpioConnectionDriver gpioConnectionDriver);
    }
}