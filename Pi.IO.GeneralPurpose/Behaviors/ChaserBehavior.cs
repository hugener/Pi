using System.Collections.Generic;
using Pi.System.Threading;

namespace Pi.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents a chaser behavior.
    /// </summary>
    public class ChaserBehavior : PinsBehavior
    {
        private bool wayOut;
        private bool roundTrip;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaserBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public ChaserBehavior(IEnumerable<PinConfiguration> configurations, IThreadFactory threadFactory = null) 
            : base(configurations, ThreadFactory.EnsureThreadFactory(threadFactory))
        {
            this.Width = 1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to roundtrip.
        /// </summary>
        /// <value>
        ///   <c>true</c> if roundtrip is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool RoundTrip
        {
            get => this.roundTrip;
            set
            {
                this.roundTrip = value;
                this.wayOut = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChaserBehavior"/> must loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets the width of the enlightned leds.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>
        /// The first step.
        /// </returns>
        protected override int GetFirstStep()
        {
            this.wayOut = true;
            return this.WidthBefore;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            var minEnabledStep = step - this.WidthBefore;
            var maxEnabledStep = step + this.WidthAfter;

            for (var i = 0; i < this.Configurations.Length; i++)
            {
                var configuration = this.Configurations[i];
                if (!this.Overflow)
                {
                    this.Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep);
                }
                else
                {
                    this.Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep) || 
                        (maxEnabledStep >= this.Configurations.Length && i <= maxEnabledStep% this.Configurations.Length) ||
                        (minEnabledStep < 0 && i >= minEnabledStep + this.Configurations.Length);
                }
            }
        }

        /// <summary>
        /// Tries to get the next step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        ///   <c>true</c> if the behavior may continue; otherwise behavior will be stopped.
        /// </returns>
        protected override bool TryGetNextStep(ref int step)
        {
            if (this.wayOut)
            {
                if (step == this.MaximumStep)
                {
                    if (this.RoundTrip)
                    {
                        this.wayOut = false;
                        step--;
                    }
                    else if (this.Loop)
                    {
                        step = this.MinimumStep;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    step++;
                }
            }
            else
            {
                if (step == this.MinimumStep)
                {
                    if (this.Loop && this.RoundTrip)
                    {
                        this.wayOut = true;
                        step++;
                    }
                    else if (this.Loop)
                    {
                        step = this.MaximumStep;
                    }
                    else
                    {
                        return false;
                    }
                }
                else step--;
            }

            return true;
        }

        private bool Overflow => this.Loop && !this.RoundTrip;

        private int MinimumStep => this.Overflow ? 0 : this.WidthBefore;

        private int MaximumStep => this.Configurations.Length - 1 - (this.Overflow ? 0 : this.WidthAfter);

        private int WidthBefore => (this.Width%2) == 1 ? (this.Width - 1)/2 : this.Width/2;

        private int WidthAfter => (this.Width%2) == 1 ? (this.Width - 1)/2 : this.Width/2 - 1;
    }
}