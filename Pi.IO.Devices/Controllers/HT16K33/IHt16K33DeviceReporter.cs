// <copyright file="IHt16K33DeviceReporter.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.HT16K33
{
    /// <summary>
    /// Reports status from <see cref="Ht16K33Device"/>.
    /// </summary>
    public interface IHt16K33DeviceReporter
    {
        /// <summary>
        /// Resettings this instance.
        /// </summary>
        void Resetting();
    }
}