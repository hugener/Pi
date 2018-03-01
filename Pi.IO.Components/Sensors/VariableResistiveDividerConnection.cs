// <copyright file="VariableResistiveDividerConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Represents a connection to an analog value coming from a resistive voltage divider
    /// </summary>
    public class VariableResistiveDividerConnection : IDisposable
    {
        private readonly IInputAnalogPin analogPin;
        private readonly Func<AnalogValue, ElectricResistance> resistorEvalFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableResistiveDividerConnection"/> class.
        /// </summary>
        /// <param name="analogPin">The analog pin.</param>
        /// <param name="resistorEvalFunc">The resistor eval function.</param>
        /// <remarks>Methods from <see cref="ResistiveDivider"/> should be used.</remarks>
        public VariableResistiveDividerConnection(IInputAnalogPin analogPin, Func<AnalogValue, ElectricResistance> resistorEvalFunc)
        {
            this.analogPin = analogPin;
            this.resistorEvalFunc = resistorEvalFunc;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the electric resistance.
        /// </summary>
        /// <returns>The resistance value.</returns>
        public ElectricResistance GetResistance()
        {
            var value = this.analogPin.Read();
            return this.resistorEvalFunc(value);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.analogPin.Dispose();
        }
    }
}
