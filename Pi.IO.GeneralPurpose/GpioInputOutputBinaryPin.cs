// <copyright file="GpioInputOutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents a bidirectional pin on GPIO interface.
    /// </summary>
    public class GpioInputOutputBinaryPin : IInputOutputBinaryPin
    {
        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;
        private readonly PinResistor resistor;
        private PinDirection? direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioInputOutputBinaryPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public GpioInputOutputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;
            this.resistor = resistor;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.direction.HasValue)
            {
                this.driver.Release(this.pin);
            }
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>The value.</returns>
        public bool Read()
        {
            this.SetDirection(PinDirection.Input);
            return this.driver.Read(this.pin);
        }

        /// <summary>
        /// Prepares the pin to act as an input.
        /// </summary>
        public void AsInput()
        {
            this.SetDirection(PinDirection.Input);
        }

        /// <summary>
        /// Prepares the pin to act as an output.
        /// </summary>
        public void AsOutput()
        {
            this.SetDirection(PinDirection.Output);
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero"/>.</param>
        /// <remarks>If <c>timeout</c> is set to <see cref="TimeSpan.Zero"/>, a default timeout is used instead.</remarks>
        public void Wait(bool waitForUp = true, TimeSpan timeout = default(TimeSpan))
        {
            this.SetDirection(PinDirection.Input);
            this.driver.Wait(this.pin, waitForUp, timeout);
        }

        /// <summary>
        /// Writes the specified state.
        /// </summary>
        /// <param name="state">the state.</param>
        public void Write(bool state)
        {
            this.SetDirection(PinDirection.Output);
            this.driver.Write(this.pin, state);
        }

        private void SetDirection(PinDirection newDirection)
        {
            if (this.direction == newDirection)
            {
                return;
            }

            if (this.direction.HasValue)
            {
                this.driver.Release(this.pin);
            }

            this.driver.Allocate(this.pin, newDirection);
            if (newDirection == PinDirection.Input
                && this.resistor != PinResistor.None
                && (this.driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) != GpioConnectionDriverCapabilities.None)
            {
                this.driver.SetPinResistor(this.pin, this.resistor);
            }

            this.direction = newDirection;
        }
    }
}