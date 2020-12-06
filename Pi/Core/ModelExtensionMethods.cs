// <copyright file="ModelExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core
{
    using System;

    /// <summary>
    /// Provides extension methods for <see cref="Model"/>.
    /// </summary>
    public static class ModelExtensionMethods
    {
        /// <summary>
        /// Gets the model display name.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The display name, if known; otherwise, <c>null</c>.</returns>
        public static string GetDisplayName(this Model model)
        {
            switch (model)
            {
                case Model.Unknown:
                    return null;
                case Model.A:
                    return "Raspberry Pi Model A";
                case Model.APlus:
                    return "Raspberry Pi Model A+";
                case Model.BRev1:
                    return "Raspberry Pi Model B rev1";
                case Model.BRev2:
                    return "Raspberry Pi Model B rev2";
                case Model.BPlus:
                    return "Raspberry Pi Model B+";
                case Model.ComputeModule:
                    return "Raspberry Pi Compute Module";
                case Model.B2:
                    return "Raspberry Pi 2 Model B";
                case Model.Zero:
                    return "Raspberry Pi Zero";
                case Model.B3:
                    return "Raspberry Pi 3 Model B";

                default:
                    throw new ArgumentOutOfRangeException(nameof(model));
            }
        }
    }
}