using System.Collections.Generic;
using System.Linq;
using Pi.System.Threading;

namespace Pi.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents a pattern behavior.
    /// </summary>
    public class PatternBehavior : PinsBehavior
    {
        private bool wayOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<int> patterns, IThreadFactory threadFactory = null)
            : this(configurations, patterns.Select(i => (long)i), threadFactory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<long> patterns, IThreadFactory threadFactory = null)
            : base(configurations, ThreadFactory.EnsureThreadFactory(threadFactory))
        {
            this.Patterns = patterns.ToArray();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PatternBehavior"/> must loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to roundtrip.
        /// </summary>
        /// <value>
        ///   <c>true</c> if round-trip is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool RoundTrip { get; set; }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>
        /// The first step.
        /// </returns>
        protected override int GetFirstStep()
        {
            this.wayOut = true;
            return 0;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            var pattern = this.Patterns[step];

            for (var i = 0; i < this.Configurations.Length; i++)
            {
                this.Connection[this.Configurations[i]] = ((pattern >> i) & 0x1) == 0x1;
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
                if (step == this.Patterns.Length - 1)
                {
                    if (this.RoundTrip)
                    {
                        this.wayOut = false;
                        step--;
                    }
                    else if (this.Loop)
                    {
                        step = 0;
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
                if (step == 0)
                {
                    if (this.Loop && this.RoundTrip)
                    {
                        this.wayOut = true;
                        step++;
                    }
                    else if (this.Loop)
                    {
                        step = this.Patterns.Length - 1;
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

        private long[] Patterns { get; }
    }
}