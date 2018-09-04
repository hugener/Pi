// <copyright file="PiFaceInputPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Hats.PiFaceDigital
{
    using global::System.Collections.Generic;

    /// <summary>
    /// Represents a PiFace input pin.
    /// </summary>
    /// <seealso cref="PiFacePin" />
    public class PiFaceInputPin : PiFacePin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceInputPin" /> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7.</param>
        internal PiFaceInputPin(int pinNumber)
            : base(pinNumber)
        {
        }

        /// <summary>
        /// Event fired when the state of a pin changes
        /// </summary>
        public event InputPinChangedHandler OnStateChanged;

        /// <summary>
        /// Gets a value indicating whether the sensor grounding the input pin is closed.
        /// </summary>
        public bool IsGrounded => !this.State;

        /// <summary>
        /// helper to set the state of every pin in a collection.
        /// </summary>
        /// <param name="inputPins">The input pins.</param>
        /// <param name="allPinState">State of all pin.</param>
        internal static void SetAllPinStates(IEnumerable<PiFaceInputPin> inputPins, byte allPinState)
        {
            foreach (var inputPin in inputPins)
            {
                inputPin.Update(allPinState);
            }
        }

        /// <summary>
        /// Update the state of this pin based on a byte that contains the state of every pin.
        /// </summary>
        /// <param name="allPinState">byte with all pin values.</param>
        internal override void Update(byte allPinState)
        {
            var oldState = this.State;
            this.State = (allPinState & this.Mask) > 0;
            if (oldState != this.State)
            {
                this.OnStateChanged?.Invoke(this, new InputPinChangedArgs(this));
            }
        }
    }
}
