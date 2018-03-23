// <copyright file="IPca9685DeviceReporter.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Pca9685
{
    using UnitsNet;

    /// <summary>
    /// Reports status from <see cref="Pca9685Device"/>.
    /// </summary>
    public interface IPca9685DeviceReporter
    {
        /// <summary>
        /// Resettings this instance.
        /// </summary>
        void Resetting();

        /// <summary>
        /// Settings the frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        void SettingFrequency(Frequency frequency);

        /// <summary>
        /// Estimateds the premaximum.
        /// </summary>
        /// <param name="preScale">The pre scale.</param>
        void EstimatedPremaximum(decimal preScale);

        /// <summary>
        /// Finals the premaximum.
        /// </summary>
        /// <param name="preScale">The pre scale.</param>
        void FinalPremaximum(decimal preScale);
    }
}