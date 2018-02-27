﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pi.System.Threading;
using Pi.Timers;

namespace Pi.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents the pins behavior base class.
    /// </summary>
    public abstract class PinsBehavior : IDisposable
    {
        private readonly ITimer timer;
        private readonly IThread thread;
        private int currentStep;

        /// <summary>
        /// Initializes a new instance of the <see cref="PinsBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="threadFactory">The thread factory.</param>
        protected PinsBehavior(IEnumerable<PinConfiguration> configurations, IThreadFactory threadFactory)
        {
            this.Configurations = configurations.ToArray();
            this.thread = threadFactory.Create();

            this.timer = Timer.Create();
            this.timer.Interval = TimeSpan.FromMilliseconds(250);
            this.timer.Tick += this.OnTimer;
        }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        public PinConfiguration[] Configurations { get; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval
        {
            get => this.timer.Interval;
            set => this.timer.Interval = value;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Timer.Dispose(this.timer);
            this.thread.Dispose();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        protected GpioConnection Connection { get; private set; }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>The first step.</returns>
        protected abstract int GetFirstStep();

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected abstract void ProcessStep(int step);

        /// <summary>
        /// Tries to get the next step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns><c>true</c> if the behavior may continue; otherwise behavior will be stopped.</returns>
        protected abstract bool TryGetNextStep(ref int step);

        internal void Start(GpioConnection connection)
        {
            this.Connection = connection;
            foreach (var pinConfiguration in this.Configurations)
            {
                connection[pinConfiguration] = false;
            }

            this.currentStep = this.GetFirstStep();
            this.timer.Start(TimeSpan.Zero);
        }

        internal void Stop()
        {
            this.timer.Stop();

            foreach (var pinConfiguration in this.Configurations)
            {
                this.Connection[pinConfiguration] = false;
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            this.ProcessStep(this.currentStep);
            if (!this.TryGetNextStep(ref this.currentStep))
            {
                this.thread.Sleep(this.Interval);
                this.Stop();
            }
        }
    }
}