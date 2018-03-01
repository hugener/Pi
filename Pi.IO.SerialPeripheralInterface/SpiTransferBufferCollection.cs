// <copyright file="SpiTransferBufferCollection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.Linq;

    /// <summary>
    /// A collection of transfer buffers that can be used to read from / write to the SPI bus.
    /// </summary>
    public class SpiTransferBufferCollection : ISpiTransferBufferCollection
    {
        private ISpiTransferBuffer[] transferBuffers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiTransferBufferCollection"/> class.
        /// </summary>
        /// <param name="numberOfMessages">Number of tranfer messages</param>
        /// <param name="messageLengthInBytes">Message size in bytes</param>
        /// <param name="transferMode">Transfer mode</param>
        public SpiTransferBufferCollection(int numberOfMessages, int messageLengthInBytes, SpiTransferMode transferMode)
        {
            if (numberOfMessages <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfMessages", numberOfMessages, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid number of messages", numberOfMessages));
            }

            this.transferBuffers = new ISpiTransferBuffer[numberOfMessages];
            for (var i = 0; i < numberOfMessages; i++)
            {
                this.transferBuffers[i] = new SpiTransferBuffer(messageLengthInBytes, transferMode);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpiTransferBufferCollection"/> class.
        /// </summary>
        ~SpiTransferBufferCollection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the number of <see cref="ISpiTransferBuffer"/> structures.
        /// </summary>
        public int Length => this.transferBuffers.Length;

        /// <summary>
        /// Can be used to request a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        public ISpiTransferBuffer this[int index] => this.transferBuffers[index];

        /// <summary>
        /// Disposes the collection of <see cref="ISpiTransferBuffer"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A method that returns a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        public ISpiTransferBuffer Get(int index)
        {
            return this.transferBuffers[index];
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<ISpiTransferBuffer> GetEnumerator()
        {
            return this.transferBuffers.OfType<ISpiTransferBuffer>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Dispose the collection of <see cref="ISpiTransferBuffer"/>.
        /// </summary>
        /// <param name="disposing">If <c>true</c> all transfer buffers will be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var buffer in this.transferBuffers)
                {
                    buffer.Dispose();
                }

                this.transferBuffers = new ISpiTransferBuffer[0];
            }
        }
    }
}