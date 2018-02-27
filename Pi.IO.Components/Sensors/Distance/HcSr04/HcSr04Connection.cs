using System;
using Pi.System.Threading;
using Pi.Timers;
using UnitsNet;

namespace Pi.IO.Components.Sensors.Distance.HcSr04
{
    /// <summary>
    /// Represents a connection to HC-SR04 distance sensor.
    /// </summary>
    /// <remarks>
    ///     <see href="https://docs.google.com/document/d/1Y-yZnNhMYy7rwhAgyL_pfa39RsB-x2qR4vP8saG73rE/edit"/> for hardware specification and 
    ///     <see href="http://www.raspberrypi-spy.co.uk/2012/12/ultrasonic-distance-measurement-using-python-part-1/"/> for implementation details.
    /// </remarks>
    public class HcSr04Connection : IDisposable
    {
        /// <summary>
        /// The default timeout (50ms).
        /// </summary>
        /// <remarks>Maximum time (if no obstacle) is 38ms.</remarks>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(50);

        private static readonly TimeSpan triggerTime = TimeSpanUtility.FromMicroseconds(10);
        private static readonly TimeSpan echoUpTimeout = TimeSpan.FromMilliseconds(500);

        private readonly IOutputBinaryPin triggerPin;
        private readonly IInputBinaryPin echoPin;
        private readonly IThread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="HcSr04Connection" /> class.
        /// </summary>
        /// <param name="triggerPin">The trigger pin.</param>
        /// <param name="echoPin">The echo pin.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public HcSr04Connection(IOutputBinaryPin triggerPin, IInputBinaryPin echoPin, IThreadFactory threadFactory = null)
        {
            this.triggerPin = triggerPin;
            this.echoPin = echoPin;
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();

            this.Timeout = DefaultTimeout;

            try
            {
                this.GetDistance();
            } catch {}
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
            this.thread.Dispose();
        }

        /// <summary>
        /// Gets or sets the timeout for distance measure.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <returns>The distance.</returns>
        public Length GetDistance()
        {
            this.triggerPin.Write(true);
            this.thread.Sleep(triggerTime);
            this.triggerPin.Write(false);

            var upTime = this.echoPin.Time(true, echoUpTimeout, this.Timeout);
            return Units.Velocity.Sound.ToDistance(upTime) / 2;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            this.triggerPin.Dispose();
            this.echoPin.Dispose();
        }
    }
}