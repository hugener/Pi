// <copyright file="I2cDriver.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    using global::System;
    using global::System.Globalization;
    using global::System.Runtime.InteropServices;
    using Pi.Core;
    using Pi.Core.Threading;
    using Pi.IO.GeneralPurpose;
    using Sundew.Base.Threading;

    /// <summary>
    /// Represents a driver for I2C devices.
    /// </summary>
    public class I2cDriver : IDisposable
    {
        private readonly object driverLock = new object();

        private readonly ProcessorPin sdaPin;
        private readonly ProcessorPin sclPin;
        private readonly ICurrentThread thread;
        private readonly bool wasSdaPinSet;
        private readonly bool wasSclPinSet;

        private readonly IntPtr gpioAddress;
        private readonly IntPtr bscAddress;

        private int currentDeviceAddress;
        private int waitInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="I2cDriver" /> class.
        /// </summary>
        /// <param name="sdaPin">The SDA pin.</param>
        /// <param name="sclPin">The SCL pin.</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <exception cref="InvalidOperationException">Unable to access device memory.</exception>
        public I2cDriver(ProcessorPin sdaPin, ProcessorPin sclPin, IThreadFactory threadFactory = null)
        {
            this.sdaPin = sdaPin;
            this.sclPin = sclPin;
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();

            var bscBase = GetBscBase(sdaPin, sclPin);

            var memoryFile = Interop.Open("/dev/mem", Interop.ORdwr + Interop.OSync);
            try
            {
                this.gpioAddress = Interop.Mmap(
                    IntPtr.Zero,
                    Interop.Bcm2835BlockSize,
                    Interop.ProtRead | Interop.ProtWrite,
                    Interop.MapShared,
                    memoryFile,
                    GetProcessorGpioAddress(Board.Current.Processor));

                this.bscAddress = Interop.Mmap(
                    IntPtr.Zero,
                    Interop.Bcm2835BlockSize,
                    Interop.ProtRead | Interop.ProtWrite,
                    Interop.MapShared,
                    memoryFile,
                    bscBase);
            }
            finally
            {
                Interop.Close(memoryFile);
            }

            if (this.bscAddress == (IntPtr)Interop.MapFailed)
            {
                throw new InvalidOperationException("Unable to access device memory");
            }

            // Set the I2C pins to the Alt 0 function to enable I2C access on them
            // remembers if the values were actually changed to clear them or not upon dispose
            this.wasSdaPinSet = this.SetPinMode((uint)(int)sdaPin, Interop.Bcm2835GpioFselAlt0); // SDA
            this.wasSclPinSet = this.SetPinMode((uint)(int)sclPin, Interop.Bcm2835GpioFselAlt0); // SCL

            // Read the clock divider register
            var dividerAddress = this.bscAddress + (int)Interop.Bcm2835BscDiv;
            var divider = (ushort)SafeReadUInt32(dividerAddress);
            this.waitInterval = GetWaitInterval(divider);

            var addressAddress = this.bscAddress + (int)Interop.Bcm2835BscA;
            SafeWriteUInt32(addressAddress, (uint)this.currentDeviceAddress);
        }

        /// <summary>
        /// Gets or sets the clock divider.
        /// </summary>
        /// <value>
        /// The clock divider.
        /// </value>
        public int ClockDivider
        {
            get
            {
                var dividerAddress = this.bscAddress + (int)Interop.Bcm2835BscDiv;
                return (ushort)SafeReadUInt32(dividerAddress);
            }

            set
            {
                var dividerAddress = this.bscAddress + (int)Interop.Bcm2835BscDiv;
                SafeWriteUInt32(dividerAddress, (uint)value);

                var actualDivider = (ushort)SafeReadUInt32(dividerAddress);
                this.waitInterval = GetWaitInterval(actualDivider);
            }
        }

        /// <summary>
        /// Connects the specified device address.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        /// <returns>The device connection.</returns>
        public I2cDeviceConnection Connect(int deviceAddress)
        {
            return new I2cDeviceConnection(this, deviceAddress);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Set all the I2C/BSC1 pins back to original values if changed
            if (this.wasSdaPinSet)
            {
                this.SetPinMode((uint)(int)this.sdaPin, Interop.Bcm2835GpioFselInpt); // SDA
            }

            if (this.wasSclPinSet)
            {
                this.SetPinMode((uint)(int)this.sclPin, Interop.Bcm2835GpioFselInpt); // SCL
            }

            Interop.Munmap(this.gpioAddress, Interop.Bcm2835BlockSize);
            Interop.Munmap(this.bscAddress, Interop.Bcm2835BlockSize);
        }

        /// <summary>
        /// Executes the specified transaction.
        /// </summary>
        /// <param name="deviceAddress">The address of the device.</param>
        /// <param name="transaction">The transaction.</param>
        internal void Execute(int deviceAddress, I2CTransaction transaction)
        {
            lock (this.driverLock)
            {
                var control = this.bscAddress + (int)Interop.Bcm2835BscC;

                foreach (I2CAction action in transaction.Actions)
                {
                    if (action is I2CWriteAction)
                    {
                        this.Write(deviceAddress, action.Buffer);
                    }
                    else if (action is I2CReadAction)
                    {
                        this.Read(deviceAddress, action.Buffer);
                    }
                    else
                    {
                        throw new InvalidOperationException("Only read and write transactions are allowed.");
                    }
                }

                WriteUInt32Mask(control, Interop.Bcm2835BscSDone, Interop.Bcm2835BscSDone);
            }
        }

        private static uint GetProcessorBscAddress(Processor processor)
        {
            switch (processor)
            {
                case Processor.Bcm2708:
                    return Interop.Bcm2835Bsc1Base;

                case Processor.Bcm2709:
                case Processor.Bcm2835:
                    return Interop.Bcm2836Bsc1Base;

                default:
                    throw new ArgumentOutOfRangeException("processor");
            }
        }

        private static uint GetProcessorGpioAddress(Processor processor)
        {
            switch (processor)
            {
                case Processor.Bcm2708:
                    return Interop.Bcm2835GpioBase;

                case Processor.Bcm2709:
                case Processor.Bcm2835:
                    return Interop.Bcm2836GpioBase;

                default:
                    throw new ArgumentOutOfRangeException("processor");
            }
        }

        private static int GetWaitInterval(ushort actualDivider)
        {
            // Calculate time for transmitting one byte
            // 1000000 = micros seconds in a second
            // 9 = Clocks per byte : 8 bits + ACK
            return (int)((decimal)actualDivider * 1000000 * 9 / Interop.Bcm2835CoreClkHz);
        }

        private static uint GetBscBase(ProcessorPin sdaPin, ProcessorPin sclPin)
        {
            switch (GpioConnectionSettings.ConnectorPinout)
            {
                case ConnectorPinout.Rev1:
                    if (sdaPin == ProcessorPin.Pin0 && sclPin == ProcessorPin.Pin1)
                    {
                        return Interop.Bcm2835Bsc0Base;
                    }

                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                case ConnectorPinout.Rev2:
                    if (sdaPin == ProcessorPin.Pin28 && sclPin == ProcessorPin.Pin29)
                    {
                        return Interop.Bcm2835Bsc0Base;
                    }

                    if (sdaPin == ProcessorPin.Pin2 && sclPin == ProcessorPin.Pin3)
                    {
                        return Interop.Bcm2835Bsc1Base;
                    }

                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                case ConnectorPinout.Plus:
                    if (sdaPin == ProcessorPin.Pin2 && sclPin == ProcessorPin.Pin3)
                    {
                        return GetProcessorBscAddress(Board.Current.Processor);
                    }

                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Connector pintout {0} is not supported", GpioConnectionSettings.ConnectorPinout));
            }
        }

        private static void WriteUInt32Mask(IntPtr address, uint value, uint mask)
        {
            var v = SafeReadUInt32(address);
            v = (v & ~mask) | (value & mask);
            SafeWriteUInt32(address, v);
        }

        private static uint SafeReadUInt32(IntPtr address)
        {
            // Make sure we dont return the _last_ read which might get lost
            // if subsequent code changes to a different peripheral
            unchecked
            {
                var returnValue = (uint)Marshal.ReadInt32(address);
                Marshal.ReadInt32(address);

                return returnValue;
            }
        }

        private static uint ReadUInt32(IntPtr address)
        {
            unchecked
            {
                return (uint)Marshal.ReadInt32(address);
            }
        }

        private static void SafeWriteUInt32(IntPtr address, uint value)
        {
            // Make sure we don't rely on the first write, which may get
            // lost if the previous access was to a different peripheral.
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
                Marshal.WriteInt32(address, (int)value);
            }
        }

        private static void WriteUInt32(IntPtr address, uint value)
        {
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
            }
        }

        private void EnsureDeviceAddress(int deviceAddress)
        {
            if (deviceAddress != this.currentDeviceAddress)
            {
                var addressAddress = this.bscAddress + (int)Interop.Bcm2835BscA;
                SafeWriteUInt32(addressAddress, (uint)deviceAddress);

                this.currentDeviceAddress = deviceAddress;
            }
        }

        private void Write(int deviceAddress, byte[] buffer)
        {
            this.EnsureDeviceAddress(deviceAddress);

            var len = (uint)buffer.Length;

            var dlen = this.bscAddress + (int)Interop.Bcm2835BscDlen;
            var fifo = this.bscAddress + (int)Interop.Bcm2835BscFifo;
            var status = this.bscAddress + (int)Interop.Bcm2835BscS;
            var control = this.bscAddress + (int)Interop.Bcm2835BscC;

            var remaining = len;
            var i = 0;

            // Clear FIFO
            WriteUInt32Mask(control, Interop.Bcm2835BscCClear1, Interop.Bcm2835BscCClear1);

            // Clear Status
            WriteUInt32(status, Interop.Bcm2835BscSClkt | Interop.Bcm2835BscSErr | Interop.Bcm2835BscSDone);

            // Set Data Length
            WriteUInt32(dlen, len);

            while (remaining != 0 && i < Interop.Bcm2835BscFifoSize)
            {
                WriteUInt32(fifo, buffer[i]);
                i++;
                remaining--;
            }

            // Enable device and start transfer
            WriteUInt32(control, Interop.Bcm2835BscCI2Cen | Interop.Bcm2835BscCSt);

            while ((ReadUInt32(status) & Interop.Bcm2835BscSDone) == 0)
            {
                while (remaining != 0 && (ReadUInt32(status) & Interop.Bcm2835BscSTxd) != 0)
                {
                    // Write to FIFO, no barrier
                    WriteUInt32(fifo, buffer[i]);
                    i++;
                    remaining--;
                }

                this.Wait(remaining);
            }

            if ((SafeReadUInt32(status) & Interop.Bcm2835BscSErr) != 0) //// Received a NACK
            {
                throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_NACK status");
            }

            if ((SafeReadUInt32(status) & Interop.Bcm2835BscSClkt) != 0) //// Received Clock Stretch Timeout
            {
                throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_CLKT status");
            }

            if (remaining != 0) //// Not all data is sent
            {
                throw new InvalidOperationException(string.Format("Read operation failed with BCM2835_I2C_REASON_ERROR_DATA status, missing {0} bytes", remaining));
            }
        }

        private void Read(int deviceAddress, byte[] buffer)
        {
            this.EnsureDeviceAddress(deviceAddress);

            var dlen = this.bscAddress + (int)Interop.Bcm2835BscDlen;
            var fifo = this.bscAddress + (int)Interop.Bcm2835BscFifo;
            var status = this.bscAddress + (int)Interop.Bcm2835BscS;
            var control = this.bscAddress + (int)Interop.Bcm2835BscC;

            var remaining = (uint)buffer.Length;
            uint i = 0;

            // Clear FIFO
            WriteUInt32Mask(control, Interop.Bcm2835BscCClear1, Interop.Bcm2835BscCClear1);

            // Clear Status
            WriteUInt32(status, Interop.Bcm2835BscSClkt | Interop.Bcm2835BscSErr | Interop.Bcm2835BscSDone);

            // Set Data Length
            WriteUInt32(dlen, (uint)buffer.Length);

            // Start read
            WriteUInt32(control, Interop.Bcm2835BscCI2Cen | Interop.Bcm2835BscCSt | Interop.Bcm2835BscCRead);

            while ((ReadUInt32(status) & Interop.Bcm2835BscSDone) == 0)
            {
                while ((ReadUInt32(status) & Interop.Bcm2835BscSRxd) != 0)
                {
                    // Read from FIFO, no barrier
                    buffer[i] = (byte)ReadUInt32(fifo);

                    i++;
                    remaining--;
                }

                this.Wait(remaining);
            }

            while (remaining != 0 && (ReadUInt32(status) & Interop.Bcm2835BscSRxd) != 0)
            {
                buffer[i] = (byte)ReadUInt32(fifo);
                i++;
                remaining--;
            }

            if ((SafeReadUInt32(status) & Interop.Bcm2835BscSErr) != 0) //// Received a NACK
            {
                throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_NACK status");
            }

            if ((SafeReadUInt32(status) & Interop.Bcm2835BscSClkt) != 0) //// Received Clock Stretch Timeout
            {
                throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_CLKT status");
            }

            if (remaining != 0) //// Not all data is received
            {
                throw new InvalidOperationException(string.Format("Read operation failed with BCM2835_I2C_REASON_ERROR_DATA status, missing {0} bytes", remaining));
            }
        }

        private void Wait(uint remaining)
        {
            // When remaining data is to be received, then wait for a fully FIFO
            if (remaining != 0)
            {
                this.thread.Sleep(TimeSpan.FromMilliseconds(this.waitInterval * (remaining >= Interop.Bcm2835BscFifoSize ? Interop.Bcm2835BscFifoSize : remaining) / 1000d));
            }
        }

        /// <summary>
        /// Sets the pin mode.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// True when value was changed, false otherwise.
        /// </returns>
        private bool SetPinMode(uint pin, uint mode)
        {
            // Function selects are 10 pins per 32 bit word, 3 bits per pin
            var paddr = this.gpioAddress + (int)(Interop.Bcm2835Gpfsel0 + (4 * (pin / 10)));
            var shift = (pin % 10) * 3;
            var mask = Interop.Bcm2835GpioFselMask << (int)shift;
            var value = mode << (int)shift;

            var existing = ReadUInt32(paddr) & mask;
            if (existing != value)
            {
                // Console.WriteLine($"existing is {x} masked:{x & mask} vs mask:{mask} value:{value}");
                WriteUInt32Mask(paddr, value, mask);
                return true;
            }

            return false;
        }
    }
}
