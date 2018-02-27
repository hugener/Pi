using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Pi.System.Threading;

namespace Pi.Timers
{
    /// <summary>
    /// Represents a high-resolution timer.
    /// </summary>
    public class HighResolutionTimer : ITimer
    {
        private readonly ManualResetEventSlim timerRunningEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim timerStoppedEvent = new ManualResetEventSlim(false);
        private readonly AutoResetEvent sleepAutoResetEvent = new AutoResetEvent(false);
        private readonly Thread timerThread;
        private readonly Thread timerActionThread;
        private readonly CancellationTokenSource disposeCancellationTokenSource = new CancellationTokenSource();
        private readonly BlockingCollection<Action> timerActions = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private TimeSpan delay;
        private EventHandler tick;

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
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public event EventHandler Tick
        {
            add
            {
                this.tick += value;
                this.StopIfHandlerEmpty();
            }
            remove
            {
                this.tick -= value;
                this.StopIfHandlerEmpty();
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="startDelay">The delay before the first occurence, in milliseconds.</param>
        public void Start(TimeSpan startDelay)
        {
            this.timerActions.Add(() => this.StartTimer(startDelay));
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
                    var lastActionTime = DateTime.Now;
                    if (!this.timerRunningEvent.IsSet)
                    {
                        continue;
                    }

                    var beginDelay = DateTime.Now;
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

                        this.tick?.Invoke(this, EventArgs.Empty);

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

                        var interval = DateTime.Now;
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