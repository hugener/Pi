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

    /// <summary>
    /// Represents a high-resolution timer.
    /// </summary>
    public class HighResolutionTimer : ITimer
    {
        private readonly object lockObject = new object();
        private readonly ManualResetEventSlim timerRunningEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim timerStoppedEvent = new ManualResetEventSlim(false);
        private readonly AutoResetEvent sleepAutoResetEvent = new AutoResetEvent(false);
        private readonly Thread timerThread;
        private readonly Thread timerActionThread;
        private readonly CancellationTokenSource disposeCancellationTokenSource = new CancellationTokenSource();
        private readonly BlockingCollection<Action> timerActions = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private TimeSpan delay;
        private TickEventHandler tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionTimer" /> class.
        /// </summary>
        internal HighResolutionTimer()
        {
            this.timerThread = new Thread(this.Timer);
            this.timerActionThread = new Thread(this.TimerControl);
            this.timerThread.Start();
            this.timerActionThread.Start();
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
            this.timerActions.Add(this.StopTimer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.disposeCancellationTokenSource?.Cancel();
            this.sleepAutoResetEvent.Set();
            Task.Run(() =>
            {
                this.timerThread.Join();
                this.timerActionThread.Join();
                this.disposeCancellationTokenSource.Dispose();
                this.timerRunningEvent.Dispose();
                this.timerStoppedEvent.Dispose();
                this.sleepAutoResetEvent.Dispose();
                this.tick = null;
            });
        }

        private void Timer()
        {
            var disposeCancellationToken = this.disposeCancellationTokenSource.Token;
            try
            {
                while (this.SetStoppedAndWaitForStart(disposeCancellationToken))
                {
                    if (!this.timerRunningEvent.IsSet)
                    {
                        continue;
                    }

                    if (!PiThread.Sleep(this.delay, this.sleepAutoResetEvent))
                    {
                        disposeCancellationToken.ThrowIfCancellationRequested();
                        continue;
                    }

                    while (true)
                    {
                        if (!this.timerRunningEvent.IsSet)
                        {
                            break;
                        }

                        disposeCancellationToken.ThrowIfCancellationRequested();

                        this.tick?.Invoke(this);

                        if (!this.timerRunningEvent.IsSet)
                        {
                            break;
                        }

                        disposeCancellationToken.ThrowIfCancellationRequested();

                        if (this.Interval == Timeout.InfiniteTimeSpan)
                        {
                            this.timerRunningEvent.Reset();
                            break;
                        }

                        if (!PiThread.Sleep(this.Interval, this.sleepAutoResetEvent))
                        {
                            disposeCancellationToken.ThrowIfCancellationRequested();
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void TimerControl()
        {
            var cancellationToken = this.disposeCancellationTokenSource.Token;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var timerAction = this.timerActions.Take(cancellationToken);
                    timerAction();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void StartOrRestartUnsafe(TimeSpan startDelay, TimeSpan interval)
        {
            if (this.IsEnabled)
            {
                this.timerActions.Add(this.StopTimer);
            }

            this.Interval = interval;
            this.timerActions.Add(() => this.StartTimer(startDelay));
        }

        private void StartTimer(TimeSpan startDelay)
        {
            if (this.timerRunningEvent.IsSet)
            {
                return;
            }

            this.delay = startDelay;
            this.sleepAutoResetEvent.Reset();
            this.timerRunningEvent.Set();
        }

        private void StopTimer()
        {
            this.sleepAutoResetEvent.Set();
            this.timerRunningEvent.Reset();
            this.timerStoppedEvent.Wait(this.disposeCancellationTokenSource.Token);
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