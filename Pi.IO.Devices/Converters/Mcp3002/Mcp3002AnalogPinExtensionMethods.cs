// <copyright file="Mcp3002AnalogPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Converters.Mcp3002
{
    /// <summary>
    /// Extension methods for <see cref="Mcp3002Device"/>.
    /// </summary>
    public static class Mcp3002AnalogPinExtensionMethods
    {
        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp3002InputAnalogPin In(this Mcp3002Device connection, Mcp3002Channel channel)
        {
            return new Mcp3002InputAnalogPin(connection, channel);
        }
    }
}