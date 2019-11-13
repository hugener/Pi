// <copyright file="SpiTransferBuffer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using Pi.IO.Interop;

    /// <summary>
    /// A transfer buffer used to read from / write to the SPI bus.
    /// </summary>
    public class SpiTransferBuffer : ISpiTransferBuffer
    {
        private readonly IMemory txBuf;
        private readonly IMemory rxBuf;

        private readonly SpiTransferMode mode;

        private SpiTransferControlStructure control;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiTransferBuffer" /> class.
        /// </summary>
        /// <param name="lengthInBytes">Size of data that shall be transmitted.</param>
        /// <param name="transferMode">Specifies read and/or write mode.</param>
        public SpiTransferBuffer(int lengthInBytes, SpiTransferMode transferMode)
        {
            if (transferMode == 0)
            {
                throw new ArgumentException("An appropriate transfer mode must be specified (read/write)", "transferMode");
            }

            this.mode = transferMode;

            if ((this.TransferMode & SpiTransferMode.Write) == SpiTransferMode.Write)
            {
                this.txBuf = new ManagedMemory(lengthInBytes);
            }

            if ((this.TransferMode & SpiTransferMode.Read) == SpiTransferMode.Read)
            {
                this.rxBuf = new ManagedMemory(lengthInBytes);
            }

            var txPtr = this.Tx is null
                ? 0
                : unchecked((ulong)this.Tx.Pointer.ToInt64());

            var rxPtr = this.Rx is null
                ? 0
                : unchecked((ulong)this.Rx.Pointer.ToInt64());

            this.control.Length = unchecked((uint)lengthInBytes);
            this.control.Tx = txPtr;
            this.control.Rx = rxPtr;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpiTransferBuffer" /> class.
        /// </summary>
        ~SpiTransferBuffer()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets or sets the temporary override of the device's wordsize.
        /// </summary>
        public byte BitsPerWord
        {
            get => this.control.BitsPerWord;
            set => this.control.BitsPerWord = value;
        }

        /// <summary>
        /// Gets or sets the temporary override of the device's bitrate (in Hz).
        /// </summary>
        public uint Speed
        {
            get => this.control.Speed;
            set => this.control.Speed = value;
        }

        /// <summary>
        /// Gets or sets the delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        public ushort Delay
        {
            get => this.control.Delay;
            set => this.control.Delay = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a chip select change will happen before starting the next transfer.
        /// </summary>
        public bool ChipSelectChange
        {
            get => this.control.ChipSelectChange == 1;
            set => this.control.ChipSelectChange = value
                ? (byte)1
                : (byte)0;
        }

        /// <summary>
        /// Gets or sets the Pad.
        /// </summary>
        public uint Pad
        {
            get => this.control.Pad;
            set => this.control.Pad = value;
        }

        /// <summary>
        /// Gets the transfer mode (read and/or write). <see cref="SpiTransferMode"/>.
        /// </summary>
        public SpiTransferMode TransferMode => this.mode;

        /// <summary>
        /// Gets the length of <see cref="Tx"/> and <see cref="Rx"/> buffers, in bytes.
        /// </summary>
        public int Length => unchecked((int)this.control.Length);

        /// <summary>
        /// Gets the pointer to userspace buffer with transmit data, or <c>null</c>. If no data is provided, zeroes are shifted out.
        /// </summary>
        public IMemory Tx => this.txBuf;

        /// <summary>
        /// Gets the pointer to userspace buffer for receive data, or <c>null</c>.
        /// </summary>
        public IMemory Rx => this.rxBuf;

        /// <summary>
        /// Gets the IOCTL structure that contains control information for a single SPI transfer.
        /// </summary>
        public SpiTransferControlStructure ControlStructure => this.control;

        /// <summary>
        /// Release all unmanaged memory.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        /// <param name="disposing">The memory will always be released to avoid memory leaks. If you don't want this, don't call this method (<see cref="Dispose(bool)"/>) in your derived class.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed stuff here
            }

            this.control.Rx = 0;
            this.control.Tx = 0;
            this.control.Length = 0;

            // always free managed/unmanaged memory
            if (!(this.txBuf is null))
            {
                this.txBuf.Dispose();
            }

            if (!(this.rxBuf is null))
            {
                this.rxBuf.Dispose();
            }
        }
    }
}