// <copyright file="IOutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    using System;

    /// <summary>
    /// Provides an interface for output, binary pins.
    /// </summary>
    public interface IOutputBinaryPin : IDisposable
    {
        /// <summary>
        /// Writes the value of the pin.
        /// </summary>
        /// <param name="state">if set to <c>true</c>, pin is set to high state.</param>
        void Write(bool state);
    }
}