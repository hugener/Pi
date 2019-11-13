// <copyright file="SpiSlaveSelectionContext.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;

    /// <summary>
    /// Represents a SPI slave selection context.
    /// </summary>
    public class SpiSlaveSelectionContext : IDisposable
    {
        private readonly SpiConnection connection;

        internal SpiSlaveSelectionContext(SpiConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.connection.DeselectSlave();
        }
    }
}