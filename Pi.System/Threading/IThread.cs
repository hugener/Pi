// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IThread.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Pi.System.Threading
{
    /// <summary>
    /// 
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