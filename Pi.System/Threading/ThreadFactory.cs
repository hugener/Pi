// <copyright file="ThreadFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using global::System;
    using Sundew.Base.Threading;

    /// <summary>
    /// Factory for creating a thread.
    /// </summary>
    public class ThreadFactory : IThreadFactory
    {
        private readonly Board board;
        private readonly Lazy<ICurrentThread> cachedThread;

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
                this.cachedThread = new Lazy<ICurrentThread>(this.CreateThread);
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
        /// <returns>An <see cref="ICurrentThread"/>.</returns>
        public ICurrentThread Create()
        {
            if (this.cachedThread != null)
            {
                return this.cachedThread.Value;
            }

            return this.CreateThread();
        }

        private ICurrentThread CreateThread()
        {
            if (this.board.IsRaspberryPi)
            {
                return new PiThread();
            }

            return new CurrentThread();
        }
    }
}
