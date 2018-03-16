// <copyright file="ITimerFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    /// <summary>
    /// Factory interface for <see cref="ITimer"/>.
    /// </summary>
    public interface ITimerFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="ITimer" />.
        /// </returns>
        ITimer Create();
    }
}