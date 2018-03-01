// <copyright file="ThreadFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using global::System;

    /// <summary>
    /// Factory for creating a thread.
    /// </summary>
    public class ThreadFactory : IThreadFactory
    {
        private readonly Board board;
        private readonly Lazy<IDisposableThread> cachedThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadFactory"/> class.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="isCaching">if set to <c>true</c> [is caching].</param>
        public ThreadFactory(Board board, bool isCaching)
        {
            this.board = board;
            if (isCaching)
            {
                this.cachedThread = new Lazy<IDisposableThread>(this.CreateThread);
            }
        }

        /// <summary>
        /// Ensures the thread factory.
        /// </summary>
        /// <param name="threadFactory">The thread factory.</param>
        /// <returns>The <see cref="IThreadFactory"/>.</returns>
        public static IThreadFactory EnsureThreadFactory(IThreadFactory threadFactory)
        {
            return threadFactory ?? Board.Current.ThreadFactory;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>An <see cref="IThread"/>.</returns>
        public IThread Create()
        {
            if (this.cachedThread != null)
            {
                return new CachedThread(this, this.cachedThread.Value);
            }

            return this.CreateThread();
        }

        internal void Dispose(IDisposableThread disposableThread)
        {
            disposableThread.DisposeThread();
        }

        private IDisposableThread CreateThread()
        {
            if (this.board.IsRaspberryPi)
            {
                return new PiThread(this);
            }

            return new StandardThread(this);
        }
    }
}
