// <copyright file="Tlc59711Channels.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    using global::System;
    using Pi.IO.Interop;

    /// <summary>
    /// The PWM channels of a TLC59711.
    /// </summary>
    internal sealed class Tlc59711Channels : IPwmChannels
    {
        private const int NumberOfChannels = 12;

        private readonly IMemory memory;
        private readonly int channelOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711Channels"/> class.
        /// </summary>
        /// <param name="memory">The memory.</param>
        /// <param name="offset">The offset.</param>
        public Tlc59711Channels(IMemory memory, int offset)
        {
            this.memory = memory;
            this.channelOffset = offset;
        }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public int Count => NumberOfChannels;

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
            ThrowOnInvalidChannelIndex(index);

            var offset = this.channelOffset + (index * 2);

            var high = this.memory.Read(offset);
            var low = this.memory.Read(offset + 1);

            return unchecked((ushort)((high << 8) | low));
        }

        /// <summary>
        /// Sets the PWM value at channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index.</param>
        /// <param name="value">The PWM value.</param>
        public void Set(int index, ushort value)
        {
            ThrowOnInvalidChannelIndex(index);

            var offset = this.channelOffset + (index * 2);

            this.memory.Write(offset, unchecked((byte)(value >> 8)));
            this.memory.Write(offset + 1, unchecked((byte)value));
        }

        private static void ThrowOnInvalidChannelIndex(int index)
        {
            if (index >= 0 && index < NumberOfChannels)
            {
                return;
            }

            var message = string.Format("The index must be greater or equal than 0 and lower than {0}.", NumberOfChannels);
            throw new ArgumentOutOfRangeException("index", index, message);
        }
    }
}