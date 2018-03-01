// <copyright file="GpioConnectionDriverFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using Configuration;
    using global::System;
    using global::System.Configuration;

    /// <summary>
    /// Factory for creating <see cref="IGpioConnectionDriver"/> depending on the platform capabilities.
    /// </summary>
    /// <seealso cref="Pi.IO.GeneralPurpose.IGpioConnectionDriverFactory" />
    public class GpioConnectionDriverFactory : IGpioConnectionDriverFactory
    {
        /// <summary>
        /// Gets the best driver for the specified capabilities.
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>The best driver, if found; otherwise, <c>null</c>.</returns>
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
        /// <returns>A <see cref="IGpioConnectionDriverFactory"/>.</returns>
        public static IGpioConnectionDriverFactory EnsureGpioConnectionDriverFactory(
            IGpioConnectionDriverFactory gpioConnectionDriverFactory)
        {
            return gpioConnectionDriverFactory ?? new GpioConnectionDriverFactory();
        }

        /// <summary>
        /// Creates an <see cref="IGpioConnectionDriver" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="IGpioConnectionDriver" />.
        /// </returns>
        public IGpioConnectionDriver Create()
        {
            var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
            return configurationSection != null && !string.IsNullOrEmpty(configurationSection.DriverTypeName)
                ? (IGpioConnectionDriver)Activator.CreateInstance(Type.GetType(configurationSection.DriverTypeName, true))
                : GetBestDriver(Board.Current.IsRaspberryPi ? GpioConnectionDriverCapabilities.None : GpioConnectionDriverCapabilities.CanWorkOnThirdPartyComputers);
        }
    }
}