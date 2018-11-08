// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGpioConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents a Gpio connection.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public interface IGpioConnection : IDisposable
    {
        /// <summary>
        /// Occurs when the status of a pin changed.
        /// </summary>
        event EventHandler<PinStatusEventArgs> PinStatusChanged;

        /// <summary>
        /// Gets a value indicating whether connection is opened.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connection is opened; otherwise, <c>false</c>.
        /// </value>
        bool IsOpened { get; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>
        /// The pins.
        /// </value>
        ConnectedPins Pins { get; }

        /// <summary>
        /// Gets or sets the status of the pin having the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>The value of the named pin.</returns>
        bool this[string name] { get; set; }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        bool this[ConnectorPin pin] { get; set; }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        bool this[PinConfiguration pin] { get; set; }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        bool this[ProcessorPin pin] { get; set; }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Clears pin attached to this connection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Add(PinConfiguration pin);

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(string pinName);

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(ConnectorPin pin);

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(ProcessorPin pin);

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(PinConfiguration configuration);

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        void Remove(string pinName);

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Remove(ConnectorPin pin);

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Remove(ProcessorPin pin);

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        void Remove(PinConfiguration configuration);

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        void Toggle(string pinName);

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Toggle(ProcessorPin pin);

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Toggle(ConnectorPin pin);

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        void Toggle(PinConfiguration configuration);

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <param name="duration">The duration.</param>
        void Blink(string pinName, TimeSpan duration = default);

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        void Blink(ProcessorPin pin, TimeSpan duration = default);

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        void Blink(ConnectorPin pin, TimeSpan duration = default);

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <param name="duration">The duration.</param>
        void Blink(PinConfiguration configuration, TimeSpan duration = default);
    }
}