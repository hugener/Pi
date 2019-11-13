// <copyright file="IGpioConnectionDriver.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Provides an interface for connection drivers.
    /// </summary>
    public interface IGpioConnectionDriver : IDisposable
    {
        /// <summary>
        /// Gets driver capabilities.
        /// </summary>
        /// <returns>The connection driver capabilities.</returns>
        GpioConnectionDriverCapabilities GetCapabilities();

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        void Allocate(ProcessorPin pin, PinDirection direction);

        /// <summary>
        /// Sets the pin resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        void SetPinResistor(ProcessorPin pin, PinResistor resistor);

        /// <summary>
        /// Sets the detected edges on an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="edges">The edges.</param>
        /// <remarks>By default, both edges may be detected on input pins.</remarks>
        void SetPinDetectedEdges(ProcessorPin pin, PinDetectedEdges edges);

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero"/>.</param>
        /// <remarks>If <c>timeout</c> is set to <see cref="TimeSpan.Zero"/>, a default timeout is used instead.</remarks>
        void Wait(ProcessorPin pin, bool waitForUp = true, TimeSpan timeout = default(TimeSpan));

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Release(ProcessorPin pin);

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        void Write(ProcessorPin pin, bool value);

        /// <summary>
        /// Reads the status of the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin status.</returns>
        bool Read(ProcessorPin pin);

        /// <summary>
        /// Reads the status of the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>The pins status.</returns>
        ProcessorPins Read(ProcessorPins pins);
    }
}