// <copyright file="Pcf8574PinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Pcf8574
{
    /// <summary>
    /// Provides extension methods for PCF8574 pins.
    /// </summary>
    public static class Pcf8574PinExtensionMethods
    {
        /// <summary>
        /// Creates an output binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The output pin.</returns>
        public static Pcf8574OutputBinaryPin Out(this Pcf8574I2CConnection connection, Pcf8574Pin pin)
        {
            return new Pcf8574OutputBinaryPin(connection, pin);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The input pin.</returns>
        public static Pcf8574InputBinaryPin In(this Pcf8574I2CConnection connection, Pcf8574Pin pin)
        {
            return new Pcf8574InputBinaryPin(connection, pin);
        }
    }
}