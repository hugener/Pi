// <copyright file="Mcp23017PinPolarity.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Mcp23017
{
    /// <summary>
    /// Enum for the pin polarity of <see cref="Mcp23017I2CConnection"/>.
    /// </summary>
    public enum Mcp23017PinPolarity
    {
        /// <summary>
        /// The normal
        /// </summary>
        Normal,

        /// <summary>
        /// The inverted
        /// </summary>
        Inverted
    }
}