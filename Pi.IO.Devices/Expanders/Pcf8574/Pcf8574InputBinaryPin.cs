// <copyright file="Pcf8574InputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Expanders.Pcf8574
{
    using global::System;

    /// <summary>
    /// Represents a binary intput pin on a PCF8574 I/O expander.
    /// </summary>
    public class Pcf8574InputBinaryPin : IInputBinaryPin
    {
        /// <summary>
        /// The default timeout (5 seconds).
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        private readonly Pcf8574Device connection;

        private readonly Pcf8574Pin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pcf8574InputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        public Pcf8574InputBinaryPin(Pcf8574Device connection, Pcf8574Pin pin)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetInputPin(pin, true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Reads the state of the pin.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the pin is in high state; otherwise, <c>false</c>.
        /// </returns>
        public bool Read()
        {
            return this.connection.GetPinStatus(this.pin);
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero"/>.</param>
        /// <remarks>If <c>timeout</c> is set to <see cref="TimeSpan.Zero"/>, a default timeout is used instead.</remarks>
        public void Wait(bool waitForUp = true, TimeSpan timeout = default(TimeSpan))
        {
            var startWait = DateTime.UtcNow;
            if (timeout == TimeSpan.Zero)
            {
                timeout = DefaultTimeout;
            }

            while (this.Read() != waitForUp)
            {
                if (DateTime.UtcNow - startWait >= timeout)
                {
                    throw new TimeoutException("A timeout occurred while waiting for pin status to change");
                }
            }
        }
    }
}