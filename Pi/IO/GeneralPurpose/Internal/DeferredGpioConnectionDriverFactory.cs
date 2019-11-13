// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferredGpioConnectionDriverFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Pi.IO.GeneralPurpose.Internal
{
    internal class DeferredGpioConnectionDriverFactory : IGpioConnectionDriverFactory
    {
        private readonly IGpioConnectionDriverFactory gpioConnectionDriverFactory;

        public DeferredGpioConnectionDriverFactory(IGpioConnectionDriverFactory gpioConnectionDriverFactory)
        {
            this.gpioConnectionDriverFactory = gpioConnectionDriverFactory;
        }

        public IGpioConnectionDriver Get()
        {
            return this.gpioConnectionDriverFactory.Get();
        }

        public void Dispose(IGpioConnectionDriver gpioConnectionDriver)
        {
            this.gpioConnectionDriverFactory.Dispose(gpioConnectionDriver);
        }

        public void Dispose()
        {
        }
    }
}