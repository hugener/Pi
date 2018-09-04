// <copyright file="Tlc59711ClusterChannels.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// The PWM channels of a TLC59711 device cluster.
    /// </summary>
    internal sealed class Tlc59711ClusterChannels : IPwmChannels
    {
        private readonly List<Mapping> deviceMap = new List<Mapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711ClusterChannels"/> class.
        /// </summary>
        /// <param name="devices">The devices.</param>
        /// <exception cref="ArgumentNullException">devices.</exception>
        public Tlc59711ClusterChannels(IEnumerable<ITlc59711Device> devices)
        {
            if (devices == null)
            {
                throw new ArgumentNullException("devices");
            }

            foreach (var device in devices)
            {
                if (device is null)
                {
                    continue;
                }

                for (var i = 0; i < device.Channels.Count; i++)
                {
                    this.deviceMap.Add(new Mapping(device, i));
                }
            }
        }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public int Count => this.deviceMap.Count;

        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself to modify PWM channel values.
        /// </summary>
        /// <param name="index">channel index.</param>
        /// <returns>The current PWM value from <paramref name="index"/>.</returns>
        public ushort this[int index]
        {
            get => this.Get(index);
            set => this.Set(index, value);
        }

        /// <summary>
        /// Returns the PWM value at the specified channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index.</param>
        /// <returns>The PWM value at the specified channel <paramref name="index"/>.</returns>
        public ushort Get(int index)
        {
            var mapping = this.deviceMap[index];
            return mapping.Device.Channels[mapping.ChannelIndex];
        }

        /// <summary>
        /// Sets the PWM value at channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index.</param>
        /// <param name="value">The PWM value.</param>
        public void Set(int index, ushort value)
        {
            var mapping = this.deviceMap[index];
            mapping.Device.Channels[mapping.ChannelIndex] = value;
        }

        private struct Mapping
        {
            private readonly ITlc59711Device device;
            private readonly int channelIndex;

            public Mapping(ITlc59711Device device, int channelIndex)
            {
                this.device = device;
                this.channelIndex = channelIndex;
            }

            public ITlc59711Device Device => this.device;

            public int ChannelIndex => this.channelIndex;
        }
    }
}