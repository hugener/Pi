// <copyright file="ScrollDirection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Ssd1306
{
    using global::System;

    /// <summary>
    /// Enum for the scroll direction.
    /// </summary>
    [Flags]
    public enum ScrollDirection : byte
    {
        /// <summary>
        /// The horizontal right
        /// </summary>
        HorizontalRight = 0x01,

        /// <summary>
        /// The horizontal left
        /// </summary>
        HorizontalLeft = 0x02,

        /// <summary>
        /// The vertical and horizontal right
        /// </summary>
        VerticalAndHorizontalRight = 0x04,

        /// <summary>
        /// The vertical and horizontal left
        /// </summary>
        VerticalAndHorizontalLeft = 0x05
    }
}