// <copyright file="HighResolutionTimer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Pi.System.Threading;
    using Sundew.Base.Threading;
    using Sundew.Base.Threading.Jobs;

    /// <summary>
    /// Represents a high-resolution timer.
    /// </summary>
    public class HighResolutionTimer : ITimer
    {
        private static readonly CurrentThread CurrentThread = new CurrentThread();
        private readonly object lockObject = new object();
        private readonly ManualResetEventSlim timerRunningEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim timerStoppedEvent = new ManualResetEventSlim(false);
        private readonly CancellableJob timerJob;
        private readonly ContinuousJob<BlockingCollection<Action>> timerActionJob;
        private readonly CancellationToken disposeCancellationToken;
        private CancellationTokenSource sleepCancellationTokenSource;
        private TimeSpan delay;
        private TickEventHandler tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionTimer" /> class.
        /// </summary>
        internal HighResolutionTimer()
        {
            this.timerJob = new CancellableJob(this.Timer);
            this.timerActionJob = new ContinuousJob<BlockingCollection<Action>>(this.TimerControl, new BlockingCollection<Action>(new ConcurrentQueue<Action>()));
            this.disposeCancellationToken = this.timerJob.Start().Value;
            this.timerActionJob.Start();
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public event TickEventHandler Tick
        {
            add
            {
                lock (this.lockObject)
                {
                    this.tick += value;
                    this.StopIfHandlerEmpty();
                }
            }

            remove
            {
                lock (this.lockObject)
                {
                    this.tick -= value;
                    this.StopIfHandlerEmpty();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled => this.timerRunningEvent.IsSet;

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval { get; private set; }

        public void StartOnce(TimeSpan startDelay)
        {
            lock (this.lockObject)
            {
                this.StartOrRestartUnsafe(startDelay, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public void Start(TimeSpan interval)
        {
            lock (this.lockObject)
            {
                this.StartOrRestartUnsafe(interval, interval);
            }
        }

        public void Start(TimeSpan startDelay, TimeSpan interval)
        {
            lock (this.lockObject)
            {
                this.StartOrRestartUnsafe(startDelay, interval);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.timerActionJob.State.Add(this.StopTimer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var timerJobCompletionTask = this.timerJob.StopAsync();
            var timerActionJobCompletionTask = this.timerActionJob.StopAsync();
            Task.Run(() =>
            {
                Task.WaitAll(timerJobCompletionTask, timerActionJobCompletionTask);
                this.timerRunningEvent.Dispose();
                this.timerStoppedEvent.Dispose();
                this.sleepCancellationTokenSource?.Dispose();
                this.tick = null;
            });
        }

        private void Timer(CancellationToken cancellationToken)
        {
            while (this.SetStoppedAndWaitForStart(cancellationToken))
            {
                if (!this.timerRunningEvent.IsSet)
                {
                    continue;
                }

                if (!PiThread.Sleep(this.delay, CurrentThread, this.sleepCancellationTokenSource.Token))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                }

                while (true)
                {
                    if (!this.timerRunningEvent.IsSet)
                    {
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    this.tick?.Invoke(this);

                    if (!this.timerRunningEvent.IsSet)
                    {
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (this.Interval == Timeout.InfiniteTimeSpan)
                    {
                        this.timerRunningEvent.Reset();
                        break;
                    }

                    if (!PiThread.Sleep(this.Interval, CurrentThread, this.sleepCancellationTokenSource.Token))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        break;
                    }
                }
            }
        }

        private void TimerControl(BlockingCollection<Action> actions, CancellationToken cancellationToken)
        {
            var timerAction = actions.Take(cancellationToken);
            timerAction();
        }

        private void StartOrRestartUnsafe(TimeSpan startDelay, TimeSpan interval)
        {
            if (this.IsEnabled)
            {
                this.timerActionJob.State.Add(this.StopTimer);
            }

            this.Interval = interval;
            this.timerActionJob.State.Add(() => this.StartTimer(startDelay));
        }

        private void StartTimer(TimeSpan startDelay)
        {
            if (this.timerRunningEvent.IsSet)
            {
                return;
            }

            this.delay = startDelay;
            this.sleepCancellationTokenSource?.Dispose();
            this.sleepCancellationTokenSource = new CancellationTokenSource();
            this.timerRunningEvent.Set();
        }

        private void StopTimer()
        {
            this.sleepCancellationTokenSource?.Cancel();
            this.timerRunningEvent.Reset();
            this.timerStoppedEvent.Wait(this.disposeCancellationToken);
        }

        private bool SetStoppedAndWaitForStart(CancellationToken disposeCancellationToken)
        {
            this.timerStoppedEvent.Set();
            return this.timerRunningEvent.Wait(Timeout.InfiniteTimeSpan, disposeCancellationToken);
        }

        private void StopIfHandlerEmpty()
        {
            if (this.tick == null)
            {
                this.Stop();
            }
        }
    }
}