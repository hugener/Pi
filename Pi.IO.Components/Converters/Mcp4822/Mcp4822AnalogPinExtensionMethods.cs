// <copyright file="Mcp4822AnalogPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Converters.Mcp4822
{
    /// <summary>
    /// Extension methods for <see cref="Mcp4822SpiConnection"/>.
    /// </summary>
    public static class Mcp4822AnalogPinExtensionMethods
    {
        /// <summary>
        /// Creates an output analog pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp4822OutputAnalogPin Out(this Mcp4822SpiConnection connection, Mcp4822Channel channel)
        {
            return new Mcp4822OutputAnalogPin(connection, channel);
        }
    }
}