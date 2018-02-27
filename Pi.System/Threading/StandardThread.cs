// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardThread.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Pi.System.Threading
{
    internal class StandardThread : IDisposableThread
    {
        private readonly ThreadFactory threadFactory;

        public StandardThread(ThreadFactory threadFactory)
        {
            this.threadFactory = threadFactory;
        }

        public void Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
        }

        public void Dispose()
        {
            this.threadFactory.Dispose(this);
        }

        void IDisposableThread.DisposeThread()
        {
        }
    }
}