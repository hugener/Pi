using System;

namespace Pi.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Defines an I2C data transaction.
    /// </summary>
    public class I2cTransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cTransaction"/> class.
        /// </summary>
        /// <param name="actions">The actions which should be performed within the transaction.</param>
        public I2cTransaction(params I2cAction[] actions)
        {
            this.Actions = actions ?? throw new ArgumentNullException("actions");
        }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        public I2cAction[] Actions { get; }
    }
}
