// <copyright file="GpioConnectionDriverFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using System;
    using global::System.Collections.Generic;
    using Pi.Core;
    using Pi.IO.GeneralPurpose.Internal;

    /// <summary>
    /// Factory for creating <see cref="IGpioConnectionDriver"/> depending on the platform capabilities.
    /// </summary>
    /// <seealso cref="Pi.IO.GeneralPurpose.IGpioConnectionDriverFactory" />
    public class GpioConnectionDriverFactory : IGpioConnectionDriverFactory
    {
        private readonly LinkedList<IGpioConnectionDriver> gpioConnectionDrivers = new LinkedList<IGpioConnectionDriver>();
        private readonly IGpioConnectionDriver gpioConnectionDriver;
        private readonly bool shouldDisposeDriver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionDriverFactory" /> class.
        /// </summary>
        public GpioConnectionDriverFactory()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionDriverFactory" /> class.
        /// </summary>
        /// <param name="useSingleton">if set to <c>true</c> [use singleton].</param>
        public GpioConnectionDriverFactory(bool useSingleton)
        {
            if (useSingleton)
            {
                this.gpioConnectionDriver = GetBestDriver(
                    Board.Current.IsRaspberryPi ? GpioConnectionDriverCapabilities.None : GpioConnectionDriverCapabilities.CanWorkOnThirdPartyComputers);
            }

            this.shouldDisposeDriver = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionDriverFactory"/> class.
        /// </summary>
        /// <param name="gpioConnectionDriver">The gpio connection driver.</param>
        public GpioConnectionDriverFactory(IGpioConnectionDriver gpioConnectionDriver)
        {
            this.gpioConnectionDriver = gpioConnectionDriver;
        }

        /// <summary>
        /// Gets the best driver for the specified capabilities.
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>
        /// The best driver, if found; otherwise, <c>null</c>.
        /// </returns>
        public static IGpioConnectionDriver GetBestDriver(GpioConnectionDriverCapabilities capabilities)
        {
            if ((GpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
            {
                return new GpioConnectionDriver();
            }

            if ((MemoryGpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
            {
                return new MemoryGpioConnectionDriver();
            }

            if ((FileGpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
            {
                return new FileGpioConnectionDriver();
            }

            return null;
        }

        /// <summary>
        /// Ensures the gpio connection driver factory.
        /// </summary>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <returns>
        /// A <see cref="IGpioConnectionDriverFactory" />.
        /// </returns>
        public static IGpioConnectionDriverFactory EnsureGpioConnectionDriverFactory(
            IGpioConnectionDriverFactory gpioConnectionDriverFactory)
        {
            if (gpioConnectionDriverFactory != null)
            {
                return new DeferredGpioConnectionDriverFactory(gpioConnectionDriverFactory);
            }

            return new GpioConnectionDriverFactory();
        }

        /// <summary>
        /// Gets or creates an <see cref="IGpioConnectionDriver" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="IGpioConnectionDriver" />.
        /// </returns>
        public IGpioConnectionDriver Get()
        {
            if (this.gpioConnectionDriver != null)
            {
                return this.gpioConnectionDriver;
            }

            var driver = GetBestDriver(Board.Current.IsRaspberryPi ? GpioConnectionDriverCapabilities.None : GpioConnectionDriverCapabilities.CanWorkOnThirdPartyComputers);
            this.gpioConnectionDrivers.AddLast(driver);
            return driver;
        }

        /// <summary>
        /// Disposes all the specified gpio connection drivers created by this factory.
        /// </summary>
        public void Dispose()
        {
            foreach (var gpioConnectionDriver in this.gpioConnectionDrivers)
            {
                gpioConnectionDriver.Dispose();
            }

            this.gpioConnectionDrivers.Clear();
            if (this.shouldDisposeDriver)
            {
                this.gpioConnectionDriver?.Dispose();
            }
        }

        /// <summary>
        /// Disposes the specified gpio connection driver if created by the factory.
        /// </summary>
        /// <param name="gpioConnectionDriver">The gpio connection driver.</param>
        public void Dispose(IGpioConnectionDriver gpioConnectionDriver)
        {
            if (this.gpioConnectionDrivers.Remove(gpioConnectionDriver))
            {
                gpioConnectionDriver.Dispose();
            }
        }
    }
}