using System;

namespace Pi.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Abstract i2c action.
    /// </summary>
    public abstract class I2cAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cAction" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        protected I2cAction(byte[] buffer)
        {
            this.Buffer = buffer ?? throw new ArgumentNullException("buffer");
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>
        /// The buffer.
        /// </value>
        public byte[] Buffer { get; }
    }
}
