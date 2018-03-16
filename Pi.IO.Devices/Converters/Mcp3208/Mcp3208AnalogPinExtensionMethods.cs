// <copyright file="Mcp3208AnalogPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Converters.Mcp3208
{
    /// <summary>
    /// Extension methods for the <see cref="Mcp3208Device"/>.
    /// </summary>
    public static class Mcp3208AnalogPinExtensionMethods
    {
        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp3208InputAnalogPin In(this Mcp3208Device connection, Mcp3208Channel channel)
        {
            return new Mcp3208InputAnalogPin(connection, channel);
        }
    }
}