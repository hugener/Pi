// <copyright file="GpioInputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents a GPIO input binary pin.
    /// </summary>
    public class GpioInputBinaryPin : IInputBinaryPin
    {
        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioInputBinaryPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public GpioInputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Input);
            if ((driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) > 0)
            {
                driver.SetPinResistor(pin, resistor);
            }

            if ((driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinDetectedEdges) > 0)
            {
                driver.SetPinDetectedEdges(pin, PinDetectedEdges.Both);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.driver.Release(this.pin);
        }

        /// <summary>
        /// Reads the state of the pin.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the pin is in high state; otherwise, <c>false</c>.
        /// </returns>
        public bool Read()
        {
            return this.driver.Read(this.pin);
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero"/>.</param>
        /// <remarks>If <c>timeout</c> is set to <see cref="TimeSpan.Zero"/>, a default timeout is used instead.</remarks>
        public void Wait(bool waitForUp = true, TimeSpan timeout = default(TimeSpan))
        {
            this.driver.Wait(this.pin, waitForUp, timeout);
        }
    }
}