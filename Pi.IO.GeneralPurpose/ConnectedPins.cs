// <copyright file="ConnectedPins.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Linq;

    /// <summary>
    /// Represents connected pins.
    /// </summary>
    public class ConnectedPins : IEnumerable<ConnectedPin>
    {
        private readonly GpioConnection connection;

        internal ConnectedPins(GpioConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="ConnectedPin"/>.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The <see cref="ConnectedPin"/> based on Processor pin.</returns>
        public ConnectedPin this[ProcessorPin pin] => new ConnectedPin(this.connection, this.connection.GetConfiguration(pin));

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="ConnectedPin"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="ConnectedPin"/> based on name.</returns>
        public ConnectedPin this[string name] => new ConnectedPin(this.connection, this.connection.GetConfiguration(name));

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="ConnectedPin"/>.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The <see cref="ConnectedPin"/> based on Connector pin.</returns>
        public ConnectedPin this[ConnectorPin pin] => this[pin.ToProcessor()];

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="ConnectedPin"/>.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The <see cref="ConnectedPin"/> based on Pin configuration.</returns>
        public ConnectedPin this[PinConfiguration pin] => new ConnectedPin(this.connection, pin);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ConnectedPin> GetEnumerator()
        {
            return this.connection.Configurations.Select(c => new ConnectedPin(this.connection, c)).GetEnumerator();
        }
    }
}