// <copyright file="Mcp23008PinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Mcp23008
{
    /// <summary>
    /// Provides extension methods for MCP23008 pins.
    /// </summary>
    public static class Mcp23008PinExtensionMethods
    {
        /// <summary>
        /// Creates an output binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The output pin.</returns>
        public static Mcp23008OutputBinaryPin Out(this Mcp23008I2CConnection connection, Mcp23008Pin pin, Mcp23008PinResistor resistor = Mcp23008PinResistor.None, Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            return new Mcp23008OutputBinaryPin(connection, pin, resistor, polarity);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The input pin.</returns>
        public static Mcp23008InputBinaryPin In(this Mcp23008I2CConnection connection, Mcp23008Pin pin, Mcp23008PinResistor resistor = Mcp23008PinResistor.None, Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            return new Mcp23008InputBinaryPin(connection, pin, resistor, polarity);
        }
    }
}
