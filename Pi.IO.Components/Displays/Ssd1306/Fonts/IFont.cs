// <copyright file="IFont.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Ssd1306.Fonts
{
    /// <summary>
    /// Interface for implmenting a font for the <see cref="Ssd1306Connection"/>.
    /// </summary>
    public interface IFont
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The font data.</returns>
        byte[][] GetData();
    }
}
