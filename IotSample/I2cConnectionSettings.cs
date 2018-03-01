// <copyright file="I2CConnectionSettings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IotSample
{
    public class I2CConnectionSettings
    {
        private byte baseAddress;

        public I2CConnectionSettings(byte baseAddress)
        {
            this.baseAddress = baseAddress;
        }
    }
}
