// <copyright file="CachedThread.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using global::System;
    using global::System.Threading;

    internal class CachedThread : IThread
    {
        private static long instanceCount;
        private readonly ThreadFactory threadFactory;
        private readonly IDisposableThread thread;

        internal CachedThread(ThreadFactory threadFactory, IDisposableThread thread)
        {
            this.threadFactory = threadFactory;
            this.thread = thread;
            Interlocked.Increment(ref instanceCount);
        }

        public void Sleep(TimeSpan timeout)
        {
            this.thread.Sleep(timeout);
        }

        public void Dispose()
        {
            if (Interlocked.Decrement(ref instanceCount) == 0)
            {
                this.threadFactory.Dispose(this.thread);
            }
        }
    }
}