// --------------------------------------------------------------------------------------------------------------------
// <copyright file="II2cDeviceConnectionReporter.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Pi.IO.InterIntegratedCircuit
{
    using System;
    using Sundew.Base.Reporting;

    /// <summary>
    /// Interface for implementing an i2c driver reporter.
    /// </summary>
    /// <seealso cref="Sundew.Base.Reporting.IReporter" />
    public interface II2cDeviceConnectionReporter : IReporter
    {
        /// <summary>
        /// Connects the specified device address.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        void Connect(int deviceAddress);

        /// <summary>
        /// Read the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        void Read(Span<byte> values);

        /// <summary>
        /// Writes the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        void Wrote(Span<byte> values);

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="values">The values.</param>
        void WriteError(Exception exception, Span<byte> values);

        /// <summary>
        /// Disposeds this instance.
        /// </summary>
        void Disposed();
    }
}