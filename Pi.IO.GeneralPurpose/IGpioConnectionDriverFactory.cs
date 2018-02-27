// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGpioConnectionDriverFactory.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Interface for implementing a factory for <see cref="IGpioConnectionDriver"/>.
    /// </summary>
    public interface IGpioConnectionDriverFactory
    {
        /// <summary>
        /// Creates an <see cref="IGpioConnectionDriver"/>.
        /// </summary>
        /// <returns>A new <see cref="IGpioConnectionDriver"/>.</returns>
        IGpioConnectionDriver Create();
    }
}