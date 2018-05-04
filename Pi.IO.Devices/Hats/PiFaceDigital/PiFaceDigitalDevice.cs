// <copyright file="PiFaceDigitalDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Hats.PiFaceDigital
{
    using global::System;
    using Pi.IO.SerialPeripheralInterface;

    /// <summary>
    /// Controls a PiFace Digital device on a Raspberry Pi.
    /// Pins on the board have software counterparts to control or read state of the physical pins.
    /// Polling is required for the input pin values as the PiFace Digital has no interrupt support.
    ///
    /// This driver uses a NativeSpiConnection which requires the native SPI driver be enabled
    /// </summary>
    public class PiFaceDigitalDevice : IDisposable
    {
        private const byte IoconSeqop = 0x00;
        private const byte AllPinsInput = 0xFF;
        private const byte AllPinsOutput = 0x00;
        private const byte CmdWrite = 0x40;
        private const byte CmdRead = 0x41;

        private readonly string driverName;

        private INativeSpiConnection spiConnection = null;

        /// <summary>
        /// Re-usable buffer for reading input pins state to reduce the polling overhead
        /// </summary>
        private ISpiTransferBuffer inputPollBuffer;

        /// <summary>
        /// Last known state of the inputs, used to optimize detecting changes
        /// </summary>
        private byte cachedInputState;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceDigitalDevice"/> class.
        /// </summary>
        /// <param name="driverName">Name of the driver.</param>
        public PiFaceDigitalDevice(string driverName = "/dev/spidev0.0")
        {
            this.driverName = driverName;
            this.InitPiFace();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceDigitalDevice"/> class.
        /// </summary>
        /// <param name="nativeSpiConnection">The native spi connection.</param>
        public PiFaceDigitalDevice(INativeSpiConnection nativeSpiConnection)
        {
            this.spiConnection = nativeSpiConnection;
            this.InitPiFace();
        }

        /// <summary>
        /// Registers on the MCP23S17 chip
        /// </summary>
        internal enum Mcp23S17Register
        {
            Iodira = 0x00,
            Iodirb = 0x01,
            Iocon = 0x0A,
            Gppub = 0x0D,
            Gpioa = 0x12,
            Gpiob = 0x13,
        }

        /// <summary>
        /// Gets the software proxy for the input pins of the PiFace Digital Board
        /// </summary>
        public PiFaceInputPin[] InputPins { get; private set; }

        /// <summary>
        /// Gets the software proxy for the output pins of the PiFace Digital Board
        /// </summary>
        public PiFaceOutputPin[] OutputPins { get; private set; }

        /// <summary>
        /// Update PiFace board with the current vales of the software output pins
        /// </summary>
        public void UpdatePiFaceOutputPins()
        {
            this.Write(Mcp23S17Register.Gpioa, PiFacePin.AllPinState(this.OutputPins));
        }

        /// <summary>
        /// Configure the output pins with a byte that has one bit per pin and set the pins on the PiFaceDigital
        /// </summary>
        /// <param name="allPinsState">State of all pins.</param>
        public void SetAllOutputPins(byte allPinsState)
        {
            foreach (var oPin in this.OutputPins)
            {
                oPin.Update(allPinsState);
            }

            this.Write(Mcp23S17Register.Gpioa, allPinsState);
        }

        /// <summary>
        /// Read the state of the input pins. Will trigger any Onchanged events registered
        /// </summary>
        public void PollInputPins()
        {
            var result = this.spiConnection.Transfer(this.inputPollBuffer);
            var state = this.inputPollBuffer.Rx[2];
            if (state != this.cachedInputState)
            {
                this.cachedInputState = state;
                PiFaceInputPin.SetAllPinStates(this.InputPins, state);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        internal byte Read(Mcp23S17Register port)
        {
            ISpiTransferBuffer transferBuffer = this.spiConnection.CreateTransferBuffer(3, SpiTransferMode.ReadWrite);
            transferBuffer.Tx[0] = CmdRead;
            transferBuffer.Tx[1] = (byte)port;
            transferBuffer.Tx[2] = 0;
            var result = this.spiConnection.Transfer(transferBuffer);
            return transferBuffer.Rx[2];
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.spiConnection.Dispose();
                this.inputPollBuffer.Dispose();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Sets up the transfer buffer for reading input pins;
        /// </summary>
        private void CreateReusableBufferForInputPolling()
        {
            this.inputPollBuffer = this.spiConnection.CreateTransferBuffer(3, SpiTransferMode.ReadWrite);
            this.inputPollBuffer.Tx[0] = CmdRead;
            this.inputPollBuffer.Tx[1] = (byte)Mcp23S17Register.Gpiob;
            this.inputPollBuffer.Tx[2] = 0;
        }

        /// <summary>
        /// Set up the MCP23S17 for the PiFace Digital board
        /// </summary>
        private void InitPiFace()
        {
            this.InputPins = new PiFaceInputPin[8];
            for (int pinNo = 0; pinNo < 8; pinNo++)
            {
                this.InputPins[pinNo] = new PiFaceInputPin(pinNo);
            }

            this.OutputPins = new PiFaceOutputPin[8];
            for (int pinNo = 0; pinNo < 8; pinNo++)
            {
                this.OutputPins[pinNo] = new PiFaceOutputPin(pinNo);
            }

            if (this.spiConnection == null)
            {
                SpiConnectionSettings spiSettings = new SpiConnectionSettings { BitsPerWord = 8, Delay = 1, MaxSpeed = 5000000, Mode = SpiMode.Mode0 };
                this.spiConnection = new NativeSpiConnection(this.driverName, spiSettings);
            }

            this.Write(Mcp23S17Register.Iocon, IoconSeqop);
            this.SetAllOutputPins(0);

            // initialize output and input pins
            this.Write(Mcp23S17Register.Iodira, AllPinsOutput);
            this.Write(Mcp23S17Register.Iodirb, AllPinsInput);

            // set resistor on all input pins to pull up
            this.Write(Mcp23S17Register.Gppub, 0xFF);

            // set outputs
            this.UpdatePiFaceOutputPins();

            // Create re-usable buffer for polling input pins
            this.CreateReusableBufferForInputPolling();

            // Get the initial software input pin state and compare to actual inputs
            this.cachedInputState = PiFacePin.AllPinState(this.InputPins);
            this.PollInputPins();
        }

        private int Write(Mcp23S17Register port, byte data)
        {
            ISpiTransferBuffer transferBuffer = this.spiConnection.CreateTransferBuffer(3, SpiTransferMode.Write);
            transferBuffer.Tx[0] = CmdWrite;
            transferBuffer.Tx[1] = (byte)port;
            transferBuffer.Tx[2] = data;
            var result = this.spiConnection.Transfer(transferBuffer);
            return result;
        }
    }
}
