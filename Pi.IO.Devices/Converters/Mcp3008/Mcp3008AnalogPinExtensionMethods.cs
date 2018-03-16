// <copyright file="Mcp3008AnalogPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Converters.Mcp3008
{
    /// <summary>
    /// Extension methods for <see cref="Mcp3008Device"/>.
    /// </summary>
    public static class Mcp3008AnalogPinExtensionMethods
    {
        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp3008InputAnalogPin In(this Mcp3008Device connection, Mcp3008Channel channel)
        {
            return new Mcp3008InputAnalogPin(connection, channel);
        }
    }
}