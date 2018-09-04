// <copyright file="NativeSpiConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Linq;
    using Pi.IO.Interop;

    /// <summary>
    /// Native SPI connection that communicates with Linux's userspace SPI driver (e.g. /dev/spidev0.0) using IOCTL.
    /// </summary>
    /// <seealso cref="Pi.IO.SerialPeripheralInterface.INativeSpiConnection" />
    public class NativeSpiConnection : INativeSpiConnection
    {
        internal const uint SpiIocRdMode = 0x80016b01;
        internal const uint SpiIocWrMode = 0x40016b01;
        internal const uint SpiIocRdLsbFirst = 0x80016b02;
        internal const uint SpiIocWrLsbFirst = 0x40016b02;
        internal const uint SpiIocRdBitsPerWord = 0x80016b03;
        internal const uint SpiIocWrBitsPerWord = 0x40016b03;
        internal const uint SpiIocRdMaxSpeedHz = 0x80046b04;
        internal const uint SpiIocWrMaxSpeedHz = 0x40046b04;

        private readonly ISpiControlDevice deviceFile;
        private ushort delay;
        private uint maxSpeed;
        private uint mode;
        private byte bitsPerWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFile">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        public NativeSpiConnection(ISpiControlDevice deviceFile)
        {
            this.deviceFile = deviceFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFile">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        /// <param name="settings">The settings.</param>
        public NativeSpiConnection(ISpiControlDevice deviceFile, SpiConnectionSettings settings)
            : this(deviceFile)
        {
            this.Init(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFilePath">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        /// <param name="settings">The settings.</param>
        public NativeSpiConnection(string deviceFilePath, SpiConnectionSettings settings)
            : this(new SpiControlDevice(new UnixFile(deviceFilePath, UnixFileMode.ReadWrite)), settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFilePath">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        public NativeSpiConnection(string deviceFilePath)
            : this(new SpiControlDevice(new UnixFile(deviceFilePath, UnixFileMode.ReadWrite)))
        {
        }

        /// <summary>
        /// Gets the delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        public ushort Delay => this.delay;

        /// <summary>
        /// Gets the maximum clock speed in Hz.
        /// </summary>
        public uint MaxSpeed => this.maxSpeed;

        /// <summary>
        /// Gets the SPI mode.
        /// </summary>
        public SpiMode Mode => (SpiMode)this.mode;

        /// <summary>
        /// Gets the device's wordsize.
        /// </summary>
        public byte BitsPerWord => this.bitsPerWord;

        /// <summary>
        /// Sets the <see cref="Delay"/>.
        /// </summary>
        /// <param name="delayInMicroSeconds">Delay in µsec.</param>
        public void SetDelay(ushort delayInMicroSeconds)
        {
            this.delay = delayInMicroSeconds;
        }

        /// <summary>
        /// Sets the maximum clock speed.
        /// </summary>
        /// <param name="maxSpeedInHz">The speed in Hz.</param>
        public void SetMaxSpeed(uint maxSpeedInHz)
        {
            this.maxSpeed = maxSpeedInHz;
            this.deviceFile.Control(SpiIocWrMaxSpeedHz, ref maxSpeedInHz)
                .ThrowOnPInvokeError<SetMaxSpeedException>("Can't set max speed in HZ (SPI_IOC_WR_MAX_SPEED_HZ). Error {1}: {2}");
            this.deviceFile.Control(SpiIocRdMaxSpeedHz, ref maxSpeedInHz)
                .ThrowOnPInvokeError<SetMaxSpeedException>("Can't set max speed in HZ (SPI_IOC_RD_MAX_SPEED_HZ). Error {1}: {2}");
        }

        /// <summary>
        /// Sets the device's wordsize <see cref="BitsPerWord"/>.
        /// </summary>
        /// <param name="wordSize">Bits per word.</param>
        public void SetBitsPerWord(byte wordSize)
        {
            this.bitsPerWord = wordSize;
            this.deviceFile.Control(SpiIocWrBitsPerWord, ref wordSize)
                .ThrowOnPInvokeError<SetBitsPerWordException>("Can't set bits per word (SPI_IOC_WR_BITS_PER_WORD). Error {1}: {2}");
            this.deviceFile.Control(SpiIocRdBitsPerWord, ref wordSize)
                .ThrowOnPInvokeError<SetBitsPerWordException>("Can't set bits per word (SPI_IOC_RD_BITS_PER_WORD). Error {1}: {2}");
        }

        /// <summary>
        /// Sets the <see cref="SpiMode"/>.
        /// </summary>
        /// <param name="spiMode">SPI mode.</param>
        public void SetSpiMode(SpiMode spiMode)
        {
            this.mode = (uint)spiMode;
            this.deviceFile.Control(SpiIocWrMode, ref this.mode)
                .ThrowOnPInvokeError<SetSpiModeException>("Can't set SPI mode (SPI_IOC_WR_MODE). Error {1}: {2}");
            this.deviceFile.Control(SpiIocRdMode, ref this.mode)
                .ThrowOnPInvokeError<SetSpiModeException>("Can't set SPI mode (SPI_IOC_RD_MODE). Error {1}: {2}");
        }

        /// <summary>
        /// Creates a transfer buffer of the given <see paramref="sizeInBytes"/> and copies the connection settings to it.
        /// </summary>
        /// <param name="sizeInBytes">Memory size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer.</returns>
        public ISpiTransferBuffer CreateTransferBuffer(int sizeInBytes, SpiTransferMode transferMode)
        {
            return new SpiTransferBuffer(sizeInBytes, transferMode)
            {
                BitsPerWord = this.bitsPerWord,
                Delay = this.delay,
                Speed = this.maxSpeed,
            };
        }

        /// <summary>
        /// Creates transfer buffers for <paramref name="numberOfMessages"/>.
        /// </summary>
        /// <param name="numberOfMessages">The number of messages to send.</param>
        /// <param name="messageSizeInBytes">Message size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer collection.</returns>
        public ISpiTransferBufferCollection CreateTransferBufferCollection(int numberOfMessages, int messageSizeInBytes, SpiTransferMode transferMode)
        {
            var collection = new SpiTransferBufferCollection(numberOfMessages, messageSizeInBytes, transferMode);
            foreach (var transferBuffer in collection)
            {
                transferBuffer.BitsPerWord = this.bitsPerWord;
                transferBuffer.Delay = this.delay;
                transferBuffer.Speed = this.maxSpeed;
            }

            return collection;
        }

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="buffer">The transfer buffer that contains data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        public int Transfer(ISpiTransferBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            var request = Interop.GetSpiMessageRequest(1);
            var structure = buffer.ControlStructure;
            var result = this.deviceFile.Control(request, ref structure);

            result.ThrowOnPInvokeError<SendSpiMessageException>("Can't send SPI message. Error {1}: {2}");

            return result;
        }

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="transferBuffers">The transfer buffers that contain data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        public int Transfer(ISpiTransferBufferCollection transferBuffers)
        {
            if (transferBuffers == null)
            {
                throw new ArgumentNullException("transferBuffers");
            }

            var request = Interop.GetSpiMessageRequest(transferBuffers.Length);

            var structures = transferBuffers
                .Select(buf => buf.ControlStructure)
                .ToArray();
            var result = this.deviceFile.Control(request, structures);

            result.ThrowOnPInvokeError<SendSpiMessageException>("Can't send SPI messages. Error {1}: {2}");

            return result;
        }

        /// <summary>
        /// Dispose instance and free all resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="disposing">If <c>true</c> all managed resources will be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.deviceFile.Dispose();
            }
        }

        private void Init(SpiConnectionSettings settings)
        {
            this.SetSpiMode(settings.Mode);
            this.SetBitsPerWord(settings.BitsPerWord);
            this.SetMaxSpeed(settings.MaxSpeed);
            this.SetDelay(settings.Delay);
        }
    }
}