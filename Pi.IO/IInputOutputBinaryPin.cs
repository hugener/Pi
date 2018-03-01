// <copyright file="IInputOutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    /// <summary>
    /// Provides an interface for bidirectional binary pins.
    /// </summary>
    public interface IInputOutputBinaryPin : IInputBinaryPin, IOutputBinaryPin
    {
        /// <summary>
        /// Prepares the pin to act as an input.
        /// </summary>
        void AsInput();

        /// <summary>
        /// Prepares the pin to act as an output.
        /// </summary>
        void AsOutput();
    }
}