// <copyright file="IInputAnalogPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    using System;

    /// <summary>
    /// Provides an interface for input, analog pin.
    /// </summary>
    public interface IInputAnalogPin : IDisposable
    {
        /// <summary>
        /// Reads the value of the pin.
        /// </summary>
        /// <returns>The value.</returns>
        AnalogValue Read();
    }
}