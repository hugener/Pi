// <copyright file="SpiConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using Pi.System.Threading;
    using Sundew.Base.Threading;

    /// <summary>
    /// Represents a connection to a SPI device.
    /// </summary>
    public class SpiConnection : IDisposable
    {
        private static readonly TimeSpan SyncDelay = TimeSpan.FromMilliseconds(1);

        private readonly IOutputBinaryPin clockPin;
        private readonly IOutputBinaryPin selectSlavePin;
        private readonly IInputBinaryPin misoPin;
        private readonly IOutputBinaryPin mosiPin;
        private readonly Endianness endianness;
        private readonly ICurrentThread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiConnection" /> class.
        /// </summary>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="selectSlavePin">The select slave pin.</param>
        /// <param name="misoPin">The miso pin.</param>
        /// <param name="mosiPin">The mosi pin.</param>
        /// <param name="endianness">The endianness.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public SpiConnection(
            IOutputBinaryPin clockPin,
            IOutputBinaryPin selectSlavePin,
            IInputBinaryPin misoPin,
            IOutputBinaryPin mosiPin,
            Endianness endianness = Endianness.LittleEndian,
            IThreadFactory threadFactory = null)
        {
            this.clockPin = clockPin;
            this.selectSlavePin = selectSlavePin;
            this.misoPin = misoPin;
            this.mosiPin = mosiPin;
            this.endianness = endianness;
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();

            clockPin.Write(false);
            selectSlavePin.Write(true);

            mosiPin?.Write(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.clockPin.Dispose();
            this.selectSlavePin.Dispose();
            this.mosiPin?.Dispose();
            this.misoPin?.Dispose();
        }

        /// <summary>
        /// Selects the slave device.
        /// </summary>
        /// <returns>The slave selection context.</returns>
        public SpiSlaveSelectionContext SelectSlave()
        {
            this.selectSlavePin.Write(false);
            return new SpiSlaveSelectionContext(this);
        }

        /// <summary>
        /// Synchronizes the devices.
        /// </summary>
        public void Synchronize()
        {
            this.clockPin.Write(true);
            this.thread.Sleep(SyncDelay);
            this.clockPin.Write(false);
        }

        /// <summary>
        /// Writes the specified bit to the device.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(bool data)
        {
            if (this.mosiPin == null)
            {
                throw new NotSupportedException("No MOSI pin has been provided");
            }

            this.mosiPin.Write(data);
            this.Synchronize();
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Write(byte data, int bitCount)
        {
            if (bitCount > 8)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "byte data cannot contain more than 8 bits");
            }

            this.SafeWrite(data, bitCount);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Write(ushort data, int bitCount)
        {
            if (bitCount > 16)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ushort data cannot contain more than 16 bits");
            }

            this.SafeWrite(data, bitCount);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Write(uint data, int bitCount)
        {
            if (bitCount > 32)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "uint data cannot contain more than 32 bits");
            }

            this.SafeWrite(data, bitCount);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Write(ulong data, int bitCount)
        {
            if (bitCount > 64)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ulong data cannot contain more than 64 bits");
            }

            this.SafeWrite(data, bitCount);
        }

        /// <summary>
        /// Reads a bit from the device.
        /// </summary>
        /// <returns>The bit status.</returns>
        public bool Read()
        {
            if (this.misoPin == null)
            {
                throw new NotSupportedException("No MISO pin has been provided");
            }

            this.Synchronize();
            return this.misoPin.Read();
        }

        /// <summary>
        /// Reads the specified number of bits from the device.
        /// </summary>
        /// <param name="bitCount">The bit count.</param>
        /// <returns>The read value.</returns>
        public ulong Read(int bitCount)
        {
            if (bitCount > 64)
            {
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ulong data cannot contain more than 64 bits");
            }

            ulong data = 0;
            for (var i = 0; i < bitCount; i++)
            {
                var index = this.endianness == Endianness.BigEndian
                                ? i
                                : bitCount - 1 - i;

                var bit = this.Read();
                if (bit)
                {
                    data |= 1UL << index;
                }
            }

            return data;
        }

        internal void DeselectSlave()
        {
            this.selectSlavePin.Write(true);
        }

        private void SafeWrite(ulong data, int bitCount)
        {
            for (var i = 0; i < bitCount; i++)
            {
                var index = this.endianness == Endianness.BigEndian
                                ? i
                                : bitCount - 1 - i;

                var bit = data & (1UL << index);
                this.Write(bit != 0);
            }
        }
    }
}