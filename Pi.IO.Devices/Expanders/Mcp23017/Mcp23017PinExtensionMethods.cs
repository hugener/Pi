// <copyright file="Mcp23017PinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Expanders.Mcp23017
{
    /// <summary>
    /// Provides extension methods for MCP23017 pins.
    /// </summary>
    public static class Mcp23017PinExtensionMethods
    {
        /// <summary>
        /// Creates an output binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The output pin.</returns>
        public static Mcp23017OutputBinaryPin Out(this Mcp23017Device connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017OutputBinaryPin(connection, pin, resistor, polarity);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The input pin.</returns>
        public static Mcp23017InputBinaryPin In(this Mcp23017Device connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017InputBinaryPin(connection, pin, resistor, polarity);
        }
    }
}