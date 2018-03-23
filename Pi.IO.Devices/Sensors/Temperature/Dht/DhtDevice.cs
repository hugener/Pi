// <copyright file="DhtDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Temperature.Dht
{
    using global::Pi.IO.GeneralPurpose;
    using global::Pi.System.Threading;
    using global::System;

    /// <summary>
    /// Represents a base class for connections to a DHT-11 or DHT-22 humidity / temperature sensor.
    /// </summary>
    /// <remarks>
    /// Requires a fast input/output switch (such as <see cref="MemoryGpioConnectionDriver"/>).
    /// Based on <see href="https://www.virtuabotix.com/virtuabotix-dht22-pinout-coding-guide/"/>, <see href="https://github.com/RobTillaart/Arduino/tree/master/libraries/DHTlib"/>
    /// Datasheet : <see href="http://www.micropik.com/PDF/dht11.pdf"/>.
    /// </remarks>
    public abstract class DhtDevice : IDisposable
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan BitSetUptime = new TimeSpan(10 * (26 + 70) / 2); // 26µs for "0", 70µs for "1"

        private readonly IInputOutputBinaryPin pin;
        private readonly IDhtDeviceReporter dhtDeviceReporter;
        private readonly IThread thread;
        private TimeSpan samplingInterval;
        private DateTime previousRead;
        private bool started;

        /// <summary>
        /// Initializes a new instance of the <see cref="DhtDevice" /> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c>, DHT is automatically started. Default value is <c>true</c>.</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <param name="dhtDeviceReporter">The DHT device reporter.</param>
        protected DhtDevice(IInputOutputBinaryPin pin, bool autoStart = true, IThreadFactory threadFactory = null, IDhtDeviceReporter dhtDeviceReporter = null)
        {
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            this.pin = pin;
            this.dhtDeviceReporter = dhtDeviceReporter;

            if (autoStart)
            {
                this.Start();
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DhtDevice"/> class.
        /// </summary>
        ~DhtDevice()
        {
            this.Close();
        }

        /// <summary>
        /// Gets or sets the sampling interval.
        /// </summary>
        /// <value>
        /// The sampling interval.
        /// </value>
        public TimeSpan SamplingInterval
        {
            get => this.samplingInterval != TimeSpan.Zero ? this.samplingInterval : this.DefaultSamplingInterval;
            set => this.samplingInterval = value;
        }

        /// <summary>
        /// Gets the default sampling interval.
        /// </summary>
        /// <value>
        /// The default sampling interval.
        /// </value>
        protected abstract TimeSpan DefaultSamplingInterval { get; }

        /// <summary>
        /// Gets the wakeup interval.
        /// </summary>
        /// <value>
        /// The wakeup interval.
        /// </value>
        protected abstract TimeSpan WakeupInterval { get; }

        /// <summary>
        /// Starts the DHT sensor. If not called, sensor will be automatically enabled before getting data.
        /// </summary>
        public void Start()
        {
            this.started = true;
            this.pin.Write(true);
            this.previousRead = DateTime.UtcNow;
        }

        /// <summary>
        /// Stops the DHT sensor. If not called, sensor will be automatically disabled after getting data.
        /// </summary>
        public void Stop()
        {
            this.pin.Write(false);
            this.started = false;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The DHT data.</returns>
        public DhtData GetData()
        {
            if (!this.started)
            {
                this.pin.Write(true);
                this.previousRead = DateTime.UtcNow;
            }

            DhtData data = null;
            var tryCount = 0;
            while (data == null && tryCount++ <= 10)
            {
                try
                {
                    data = this.TryGetData();
                    data.AttemptCount = tryCount;
                }
                catch (Exception ex)
                {
                    this.dhtDeviceReporter?.FailToReadData(tryCount, ex);
                }
            }

            if (!this.started)
            {
                this.pin.Write(false);
            }

            return data;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            GC.SuppressFinalize(this);
            this.pin.Dispose();
            this.thread.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the DHT data.
        /// </summary>
        /// <param name="temperatureValue">The temperature value.</param>
        /// <param name="humidityValue">The humidity value.</param>
        /// <returns>The DhtData.</returns>
        protected abstract DhtData GetDhtData(int temperatureValue, int humidityValue);

        private DhtData TryGetData()
        {
            // Prepare buffer
            var data = new byte[5];
            for (var i = 0; i < 5; i++)
            {
                data[i] = 0;
            }

            var remainingSamplingInterval = this.SamplingInterval - (DateTime.UtcNow - this.previousRead);
            if (remainingSamplingInterval > TimeSpan.Zero)
            {
                this.thread.Sleep(remainingSamplingInterval);
            }

            // Prepare for reading
            try
            {
                // Measure required by host : pull down then pull up
                this.pin.Write(false);
                this.thread.Sleep(this.WakeupInterval);
                this.pin.Write(true);

                // Read acknowledgement from DHT
                this.pin.Wait(true, Timeout);
                this.pin.Wait(false, Timeout);

                // Read 40 bits output, or time-out
                var cnt = 7;
                var idx = 0;
                for (var i = 0; i < 40; i++)
                {
                    this.pin.Wait(true, Timeout);
                    var start = DateTime.UtcNow;
                    this.pin.Wait(false, Timeout);

                    // Determine whether bit is "1" or "0"
                    if (DateTime.UtcNow - start > BitSetUptime)
                    {
                        data[idx] |= (byte)(1 << cnt);
                    }

                    if (cnt == 0)
                    {
                        idx++;      // next byte
                        cnt = 7;    // restart at MSB
                    }
                    else
                    {
                        cnt--;
                    }
                }
            }
            finally
            {
                // Prepare for next reading
                this.previousRead = DateTime.UtcNow;
                this.pin.Write(true);
            }

            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
            {
                throw new InvalidChecksumException("Invalid checksum on DHT data", data[4], checkSum & 0xff);
            }

            var sign = 1;
            if ((data[2] & 0x80) != 0) //// negative temperature
            {
                data[2] = (byte)(data[2] & 0x7F);
                sign = -1;
            }

            var humidity = (data[0] << 8) + data[1];
            var temperature = sign * ((data[2] << 8) + data[3]);

            return this.GetDhtData(temperature, humidity);
        }
    }
}