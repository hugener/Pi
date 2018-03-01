// <copyright file="Tlc59711Cluster.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Controllers.Tlc59711
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Interop;

    /// <summary>
    /// A chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711.
    /// The devices should be connected together with their SDTI/SDTO pins.
    /// </summary>
    public class Tlc59711Cluster : ITlc59711Cluster
    {
        private const int CommandSize = Tlc59711Device.CommandSize;

        private readonly ITlc59711Device[] devices;
        private readonly IPwmChannels channels;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711Cluster"/> class.
        /// </summary>
        /// <param name="memory">Memory to work with.</param>
        /// <param name="numberOfDevices">Number of <see cref="ITlc59711Device" />s connected together.</param>
        /// <exception cref="ArgumentNullException">memory</exception>
        /// <exception cref="ArgumentOutOfRangeException">numberOfDevices - You cannot create a cluster with less than one device.</exception>
        /// <exception cref="InsufficientMemoryException">Thrown in memory is insufficient.</exception>
        public Tlc59711Cluster(IMemory memory, int numberOfDevices)
        {
            if (ReferenceEquals(memory, null))
            {
                throw new ArgumentNullException("memory");
            }

            if (numberOfDevices <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfDevices", "You cannot create a cluster with less than one device.");
            }

            var minimumRequiredMemorySize = numberOfDevices * CommandSize;
            if (memory.Length < minimumRequiredMemorySize)
            {
                var message = string.Format("For {0} device(s) you have to provide a minimum of {1} bytes of memory.", numberOfDevices, minimumRequiredMemorySize);
                throw new InsufficientMemoryException(message);
            }

            this.devices = CreateDevices(memory, numberOfDevices).ToArray();
            this.channels = new Tlc59711ClusterChannels(this.devices);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711Cluster"/> class.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public Tlc59711Cluster(IEnumerable<ITlc59711Device> devices)
        {
            this.devices = devices.ToArray();
            this.channels = new Tlc59711ClusterChannels(this.devices);
        }

        /// <summary>
        /// Gets the number of TLC59711 devices chained together
        /// </summary>
        public int Count => this.devices.Length;

        /// <summary>
        /// Gets the PWM channels
        /// </summary>
        public IPwmChannels Channels => this.channels;

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        public ITlc59711Device this[int index] => this.devices[index];

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> object.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ITlc59711Device> GetEnumerator()
        {
            return ((IEnumerable<ITlc59711Device>)this.devices)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        public ITlc59711Device Get(int index)
        {
            return this.devices[index];
        }

        /// <summary>
        /// Set BLANK on/off at all connected devices.
        /// </summary>
        /// <param name="blank">If set to <c>true</c> all outputs are forced off.</param>
        public void Blank(bool blank)
        {
            foreach (var device in this.devices)
            {
                device.Blank = blank;
            }
        }

        private static IEnumerable<ITlc59711Device> CreateDevices(IMemory memory, int numberOfDevices)
        {
            for (var i = 0; i < numberOfDevices; i++)
            {
                var subset = new MemorySubset(memory, i * CommandSize, CommandSize, false);
                yield return new Tlc59711Device(subset);
            }
        }
    }
}