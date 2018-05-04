// <copyright file="InputPinChangedHandler.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Hats.PiFaceDigital
{
    /// <summary>
    /// delgate for pin state changed events. The pin that changed is in the args
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public delegate void InputPinChangedHandler(object sender, InputPinChangedArgs e);
}
