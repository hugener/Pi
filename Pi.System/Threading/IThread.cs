// <copyright file="IThread.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using global::System;

    /// <summary>
    /// Interface for implementing thread sleep.
    /// </summary>
    public interface IThread : IDisposable
    {
        /// <summary>
        /// Sleeps the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        void Sleep(TimeSpan delay);
    }
}