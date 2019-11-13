// <copyright file="GpioOutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Represents an output pin on GPIO interface.
    /// </summary>
    public class GpioOutputBinaryPin : IOutputBinaryPin
    {
        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioOutputBinaryPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public GpioOutputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Output);
            if ((driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) > 0)
            {
                driver.SetPinResistor(pin, resistor);
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
        /// Writes the specified state.
        /// </summary>
        /// <param name="state">The pin state.</param>
        public void Write(bool state)
        {
            this.driver.Write(this.pin, state);
        }
    }
}