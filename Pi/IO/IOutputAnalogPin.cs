// <copyright file="IOutputAnalogPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    using System;

    /// <summary>
    /// Provides an interface for output, analog pin.
    /// </summary>
    public interface IOutputAnalogPin : IDisposable
    {
        /// <summary>
        /// Writes the specified value to the pin.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(AnalogValue value);
    }
}