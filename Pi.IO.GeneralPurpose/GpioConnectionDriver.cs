// <copyright file="GpioConnectionDriver.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Runtime.InteropServices;
    using Pi.IO.Interop;
    using Pi.System.Threading;
    using Pi.Timers;

    /// <summary>
    /// Represents the default connection driver that uses memory for accesses and files for edge detection.
    /// </summary>
    public class GpioConnectionDriver : IGpioConnectionDriver
    {
        /// <summary>
        /// The minimum timeout (1 milliseconds).
        /// </summary>
        public static readonly TimeSpan MinimumTimeout = TimeSpan.FromMilliseconds(1);

        /// <summary>
        /// The default timeout (5 seconds).
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        private const string GpioPath = "/sys/class/gpio";

        private static readonly TimeSpan ResistorSetDelay = TimeSpanUtility.FromMicroseconds(5);

        private readonly IntPtr gpioAddress;
        private readonly Dictionary<ProcessorPin, PinResistor> pinResistors = new Dictionary<ProcessorPin, PinResistor>();
        private readonly Dictionary<ProcessorPin, PinPoll> pinPolls = new Dictionary<ProcessorPin, PinPoll>();
        private readonly IThread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionDriver" /> class.
        /// </summary>
        /// <param name="threadFactory">The thread factory.</param>
        public GpioConnectionDriver(IThreadFactory threadFactory = null)
        {
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            using (var memoryFile = UnixFile.Open("/dev/mem", UnixFileMode.ReadWrite | UnixFileMode.Synchronized))
            {
                this.gpioAddress = MemoryMap.Create(
                    IntPtr.Zero,
                    Interop.Bcm2835BlockSize,
                    MemoryProtection.ReadWrite,
                    MemoryFlags.Shared,
                    memoryFile.Descriptor,
                    GetProcessorBaseAddress(Board.Current.Processor));
            }
        }

        /// <summary>
        /// Gets driver capabilities.
        /// </summary>
        /// <returns>The capabilites.</returns>
        public static GpioConnectionDriverCapabilities GetCapabilities()
        {
            return GpioConnectionDriverCapabilities.CanSetPinResistor | GpioConnectionDriverCapabilities.CanSetPinDetectedEdges;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MemoryMap.Close(this.gpioAddress, Interop.Bcm2835BlockSize);
            this.thread.Dispose();
        }

        /// <summary>
        /// Gets driver capabilities.
        /// </summary>
        /// <returns>The capabilites.</returns>
        GpioConnectionDriverCapabilities IGpioConnectionDriver.GetCapabilities()
        {
            return GetCapabilities();
        }

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            var gpioId = string.Format("gpio{0}", (int)pin);
            if (Directory.Exists(Path.Combine(GpioPath, gpioId)))
            {
                // Reinitialize pin virtual file
                using (var streamWriter = new StreamWriter(Path.Combine(GpioPath, "unexport"), false))
                {
                    streamWriter.Write((int)pin);
                }
            }

            // Export pin for file mode
            using (var streamWriter = new StreamWriter(Path.Combine(GpioPath, "export"), false))
            {
                streamWriter.Write((int)pin);
            }

            // Set the direction on the pin and update the exported list
            this.SetPinMode(pin, direction == PinDirection.Input ? Interop.Bcm2835GpioFselInpt : Interop.Bcm2835GpioFselOutp);

            // Set direction in pin virtual file
            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(GpioPath, filePath), false))
            {
                streamWriter.Write(direction == PinDirection.Input ? "in" : "out");
            }

            if (direction == PinDirection.Input)
            {
                if (!this.pinResistors.TryGetValue(pin, out var pinResistor) || pinResistor != PinResistor.None)
                {
                    this.SetPinResistor(pin, PinResistor.None);
                }

                this.SetPinDetectedEdges(pin, PinDetectedEdges.Both);
                this.InitializePoll(pin);
            }
        }

        /// <summary>
        /// Sets the pin resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetPinResistor(ProcessorPin pin, PinResistor resistor)
        {
            // Set the pullup/down resistor for a pin
            //
            // The GPIO Pull-up/down Clock Registers control the actuation of internal pull-downs on
            // the respective GPIO pins. These registers must be used in conjunction with the GPPUD
            // register to effect GPIO Pull-up/down changes. The following sequence of events is
            // required:
            // 1. Write to GPPUD to set the required control signal (i.e. Pull-up or Pull-Down or neither
            // to remove the current Pull-up/down)
            // 2. Wait 150 cycles ? this provides the required set-up time for the control signal
            // 3. Write to GPPUDCLK0/1 to clock the control signal into the GPIO pads you wish to
            // modify ? NOTE only the pads which receive a clock will be modified, all others will
            // retain their previous state.
            // 4. Wait 150 cycles ? this provides the required hold time for the control signal
            // 5. Write to GPPUD to remove the control signal
            // 6. Write to GPPUDCLK0/1 to remove the clock
            //
            // RPi has P1-03 and P1-05 with 1k8 pullup resistor
            uint pud;
            switch (resistor)
            {
                case PinResistor.None:
                    pud = Interop.Bcm2835GpioPudOff;
                    break;
                case PinResistor.PullDown:
                    pud = Interop.Bcm2835GpioPudDown;
                    break;
                case PinResistor.PullUp:
                    pud = Interop.Bcm2835GpioPudUp;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("resistor", resistor, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for pin resistor", resistor));
            }

            this.WriteResistor(pud);
            this.thread.Sleep(ResistorSetDelay);
            this.SetPinResistorClock(pin, true);
            this.thread.Sleep(ResistorSetDelay);
            this.WriteResistor(Interop.Bcm2835GpioPudOff);
            this.SetPinResistorClock(pin, false);

            this.pinResistors[pin] = PinResistor.None;
        }

        /// <summary>
        /// Sets the detected edges on an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="edges">The edges.</param>
        /// <remarks>By default, both edges may be detected on input pins.</remarks>
        public void SetPinDetectedEdges(ProcessorPin pin, PinDetectedEdges edges)
        {
            var edgePath = Path.Combine(GpioPath, string.Format("gpio{0}/edge", (int)pin));
            using (var streamWriter = new StreamWriter(edgePath, false))
            {
                streamWriter.Write(ToString(edges));
            }
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero" />.</param>
        /// <remarks>
        /// If <c>timeout</c> is set to <see cref="TimeSpan.Zero" />, a 5 seconds timeout is used.
        /// </remarks>
        public void Wait(ProcessorPin pin, bool waitForUp = true, TimeSpan timeout = default)
        {
            var pinPoll = this.pinPolls[pin];
            if (this.Read(pin) == waitForUp)
            {
                return;
            }

            var actualTimeout = GetActualTimeout(timeout);

            while (true)
            {
                // TODO: timeout after the remaining amount of time.
                var waitResult = Interop.Epoll_wait(pinPoll.PollDescriptor, pinPoll.OutEventPtr, 1, (int)actualTimeout.TotalMilliseconds);
                if (waitResult > 0)
                {
                    if (this.Read(pin) == waitForUp)
                    {
                        break;
                    }
                }
                else if (waitResult == 0)
                {
                    throw new TimeoutException(string.Format(
                        CultureInfo.InvariantCulture,
                        "Operation timed out after waiting {0}ms for the pin {1} to be {2}",
                        actualTimeout,
                        pin,
                        waitForUp ? "up" : "down"));
                }
                else
                {
                    throw new IOException("Call to epoll_wait API failed");
                }
            }
        }

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Release(ProcessorPin pin)
        {
            this.UninitializePoll(pin);
            using (var streamWriter = new StreamWriter(Path.Combine(GpioPath, "unexport"), false))
            {
                streamWriter.Write((int)pin);
            }

            this.SetPinMode(pin, Interop.Bcm2835GpioFselInpt);
        }

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        public void Write(ProcessorPin pin, bool value)
        {
            var offset = Math.DivRem((int)pin, 32, out int shift);

            var pinGroupAddress = this.gpioAddress + (int)((value ? Interop.Bcm2835Gpset0 : Interop.Bcm2835Gpclr0) + offset);
            SafeWriteUInt32(pinGroupAddress, 1U << shift);
        }

        /// <summary>
        /// Reads the status of the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        /// The pin status.
        /// </returns>
        public bool Read(ProcessorPin pin)
        {
            var offset = Math.DivRem((int)pin, 32, out int shift);

            var pinGroupAddress = this.gpioAddress + (int)(Interop.Bcm2835Gplev0 + offset);
            var value = SafeReadUInt32(pinGroupAddress);

            return (value & (1 << shift)) != 0;
        }

        /// <summary>
        /// Reads the status of the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>
        /// The pins status.
        /// </returns>
        public ProcessorPins Read(ProcessorPins pins)
        {
            var pinGroupAddress = this.gpioAddress + (int)(Interop.Bcm2835Gplev0 + (0U * 4));
            var value = SafeReadUInt32(pinGroupAddress);

            return (ProcessorPins)((uint)pins & value);
        }

        private static uint GetProcessorBaseAddress(Processor processor)
        {
            switch (processor)
            {
                case Processor.Bcm2708:
                    return Interop.Bcm2835GpioBase;

                case Processor.Bcm2709:
                case Processor.Bcm2835:
                    return Interop.Bcm2836GpioBase;

                default:
                    throw new ArgumentOutOfRangeException("processor");
            }
        }

        private static TimeSpan GetActualTimeout(TimeSpan timeout)
        {
            if (timeout > TimeSpan.Zero)
            {
                return timeout;
            }

            if (timeout < TimeSpan.Zero)
            {
                return MinimumTimeout;
            }

            return DefaultTimeout;
        }

        private static void WriteUInt32Mask(IntPtr address, uint value, uint mask)
        {
            var v = SafeReadUInt32(address);
            v = (v & ~mask) | (value & mask);
            SafeWriteUInt32(address, v);
        }

        private static uint SafeReadUInt32(IntPtr address)
        {
            // Make sure we dont return the _last_ read which might get lost
            // if subsequent code changes to a different peripheral
            var ret = ReadUInt32(address);
            ReadUInt32(address);

            return ret;
        }

        private static uint ReadUInt32(IntPtr address)
        {
            unchecked
            {
                return (uint)Marshal.ReadInt32(address);
            }
        }

        private static void SafeWriteUInt32(IntPtr address, uint value)
        {
            // Make sure we don't rely on the first write, which may get
            // lost if the previous access was to a different peripheral.
            WriteUInt32(address, value);
            WriteUInt32(address, value);
        }

        private static void WriteUInt32(IntPtr address, uint value)
        {
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
            }
        }

        private static string ToString(PinDetectedEdges edges)
        {
            switch (edges)
            {
                case PinDetectedEdges.Both:
                    return "both";
                case PinDetectedEdges.Rising:
                    return "rising";
                case PinDetectedEdges.Falling:
                    return "falling";
                case PinDetectedEdges.None:
                    return "none";
                default:
                    throw new ArgumentOutOfRangeException("edges", edges, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for edge detection", edges));
            }
        }

        private void InitializePoll(ProcessorPin pin)
        {
            lock (this.pinPolls)
            {
                if (this.pinPolls.TryGetValue(pin, out var poll))
                {
                    return;
                }

                var pinPoll = default(PinPoll);

                pinPoll.PollDescriptor = Interop.Epoll_create(1);
                if (pinPoll.PollDescriptor < 0)
                {
                    throw new IOException("Call to epoll_create(1) API failed with the following return value: " + pinPoll.PollDescriptor);
                }

                var valuePath = Path.Combine(GpioPath, string.Format("gpio{0}/value", (int)pin));

                pinPoll.FileDescriptor = UnixFile.OpenFileDescriptor(valuePath, UnixFileMode.ReadOnly | UnixFileMode.NonBlocking);

                var ev = new Interop.EpollEvent
                {
                    Events = Interop.Epollin | Interop.Epollet | Interop.Epollpri,
                    Data = new Interop.EpollData { Fd = pinPoll.FileDescriptor },
                };

                pinPoll.InEventPtr = Marshal.AllocHGlobal(64);
                Marshal.StructureToPtr(ev, pinPoll.InEventPtr, false);

                var controlResult = Interop.Epoll_ctl(pinPoll.PollDescriptor, Interop.EpollCtlAdd, pinPoll.FileDescriptor, pinPoll.InEventPtr);
                if (controlResult != 0)
                {
                    throw new IOException("Call to epoll_ctl(EPOLL_CTL_ADD) API failed with the following return value: " + controlResult);
                }

                pinPoll.OutEventPtr = Marshal.AllocHGlobal(64);
                this.pinPolls[pin] = pinPoll;
            }
        }

        private void UninitializePoll(ProcessorPin pin)
        {
            if (this.pinPolls.TryGetValue(pin, out PinPoll poll))
            {
                this.pinPolls.Remove(pin);

                var controlResult = poll.InEventPtr != IntPtr.Zero ? Interop.Epoll_ctl(poll.PollDescriptor, Interop.EpollCtlDel, poll.FileDescriptor, poll.InEventPtr) : 0;

                Marshal.FreeHGlobal(poll.InEventPtr);
                Marshal.FreeHGlobal(poll.OutEventPtr);

                UnixFile.CloseFileDescriptor(poll.PollDescriptor);
                UnixFile.CloseFileDescriptor(poll.FileDescriptor);

                if (controlResult != 0)
                {
                    throw new IOException("Call to epoll_ctl(EPOLL_CTL_DEL) API failed with the following return value: " + controlResult);
                }
            }
        }

        private void SetPinResistorClock(ProcessorPin pin, bool on)
        {
            var offset = Math.DivRem((int)pin, 32, out int shift);

            var clockAddress = this.gpioAddress + (int)(Interop.Bcm2835Gppudclk0 + offset);
            SafeWriteUInt32(clockAddress, (uint)(on ? 1 : 0) << shift);
        }

        private void WriteResistor(uint resistor)
        {
            var resistorPin = this.gpioAddress + (int)Interop.Bcm2835Gppud;
            SafeWriteUInt32(resistorPin, resistor);
        }

        private void SetPinMode(ProcessorPin pin, uint mode)
        {
            // Function selects are 10 pins per 32 bit word, 3 bits per pin
            var pinModeAddress = this.gpioAddress + (int)(Interop.Bcm2835Gpfsel0 + (4 * ((int)pin / 10)));

            var shift = 3 * ((int)pin % 10);
            var mask = Interop.Bcm2835GpioFselMask << shift;
            var value = mode << shift;

            WriteUInt32Mask(pinModeAddress, value, mask);
        }

        private struct PinPoll
        {
            public int FileDescriptor;
            public int PollDescriptor;
            public IntPtr InEventPtr;
            public IntPtr OutEventPtr;
        }
    }
}