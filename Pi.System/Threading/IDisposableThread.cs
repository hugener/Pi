// <copyright file="IDisposableThread.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    /// <summary>
    /// An <see cref="IThread"/> that is disposable.
    /// </summary>
    /// <seealso cref="Pi.System.Threading.IThread" />
    internal interface IDisposableThread : IThread
    {
        /// <summary>
        /// Disposes the thread.
        /// </summary>
        void DisposeThread();
    }
}