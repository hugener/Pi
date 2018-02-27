// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hd44780DataPins.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Pi.IO.GeneralPurpose;

namespace Pi.IO.Components.Displays.Hd44780
{
    public class Hd44780DataPins
    {
        public Hd44780DataPins(params ConnectorPin[] connectorPins)
        {
            this.ConnectorPins = connectorPins;
        }

        public ConnectorPin[] ConnectorPins { get; }
    }
}