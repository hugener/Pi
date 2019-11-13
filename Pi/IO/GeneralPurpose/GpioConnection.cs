// <copyright file="GpioConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Pi.Core.Threading;
    using Pi.Core.Timers;
    using Sundew.Base.Threading;

    /// <summary>
    /// Represents a connection to the GPIO pins.
    /// </summary>
    public class GpioConnection : IGpioConnection
    {
        private readonly GpioConnectionSettings settings;

        private readonly Dictionary<ProcessorPin, PinConfiguration> pinConfigurations;
        private readonly Dictionary<string, PinConfiguration> namedPins;
        private readonly IGpioConnectionDriverFactory gpioConnectionDriverFactory;
        private readonly ITimer timer;
        private readonly Dictionary<ProcessorPin, bool> pinValues = new Dictionary<ProcessorPin, bool>();
        private readonly Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>> pinEvents = new Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>>();
        private readonly ICurrentThread thread;
        private readonly IGpioConnectionDriver gpioConnectionDriver;

        private ProcessorPins inputPins = ProcessorPins.None;
        private ProcessorPins pinRawValues = ProcessorPins.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="pins">The pins.</param>
        public GpioConnection(params PinConfiguration[] pins)
            : this(null, pins, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="pins">The pins.</param>
        public GpioConnection(IEnumerable<PinConfiguration> pins)
            : this(null, pins)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(GpioConnectionSettings settings, params PinConfiguration[] pins)
            : this(settings, pins, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(IGpioConnectionDriverFactory gpioConnectionDriverFactory, params PinConfiguration[] pins)
            : this(null, pins, gpioConnectionDriverFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(IGpioConnectionDriverFactory gpioConnectionDriverFactory, IEnumerable<PinConfiguration> pins)
            : this(null, pins, gpioConnectionDriverFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(GpioConnectionSettings settings, IGpioConnectionDriverFactory gpioConnectionDriverFactory, params PinConfiguration[] pins)
            : this(settings, pins, gpioConnectionDriverFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pins">The pins.</param>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public GpioConnection(GpioConnectionSettings settings, IEnumerable<PinConfiguration> pins, IGpioConnectionDriverFactory gpioConnectionDriverFactory = null, IThreadFactory threadFactory = null)
        {
            this.settings = settings ?? new GpioConnectionSettings();
            this.gpioConnectionDriverFactory = GpioConnectionDriverFactory.EnsureGpioConnectionDriverFactory(gpioConnectionDriverFactory);
            this.gpioConnectionDriver = this.gpioConnectionDriverFactory.Get();
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            this.Pins = new ConnectedPins(this);

            var pinList = pins.ToList();
            this.pinConfigurations = pinList.ToDictionary(p => p.Pin);

            this.namedPins = pinList.Where(p => !string.IsNullOrEmpty(p.Name)).ToDictionary(p => p.Name);

            this.timer = Core.Timers.Timer.Create();

            this.timer.Tick += this.CheckInputPins;

            if (this.settings.Opened)
            {
                this.Open();
            }
        }

        /// <summary>
        /// Occurs when the status of a pin changed.
        /// </summary>
        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        /// <summary>
        /// Gets a value indicating whether connection is opened.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connection is opened; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpened { get; private set; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>
        /// The pins.
        /// </value>
        public ConnectedPins Pins { get; }

        internal IEnumerable<PinConfiguration> Configurations => this.pinConfigurations.Values;

        /// <summary>
        /// Gets or sets the status of the pin having the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>The value of the named pin.</returns>
        public bool this[string name]
        {
            get => this[this.namedPins[name].Pin];
            set => this[this.namedPins[name].Pin] = value;
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        public bool this[ConnectorPin pin]
        {
            get => this[pin.ToProcessor()];
            set => this[pin.ToProcessor()] = value;
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        public bool this[PinConfiguration pin]
        {
            get => this.pinValues[pin.Pin];
            set
            {
                if (pin.Direction == PinDirection.Output)
                {
                    var pinValue = pin.GetEffective(value);
                    this.gpioConnectionDriver.Write(pin.Pin, pinValue);

                    this.pinValues[pin.Pin] = value;
                    this.OnPinStatusChanged(new PinStatusEventArgs { Enabled = value, Configuration = pin });
                }
                else
                {
                    throw new InvalidOperationException("Value of input pins cannot be modified");
                }
            }
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        /// <value>
        /// The <see cref="bool" />.
        /// </value>
        /// <param name="pin">The pin.</param>
        /// <returns>The value of the pin.</returns>
        public bool this[ProcessorPin pin]
        {
            get => this[this.pinConfigurations[pin]];
            set => this[this.pinConfigurations[pin]] = value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            Core.Timers.Timer.Dispose(this.timer);
            this.gpioConnectionDriverFactory.Dispose();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open()
        {
            lock (this.timer)
            {
                if (this.IsOpened)
                {
                    return;
                }

                foreach (var pin in this.pinConfigurations.Values)
                {
                    this.Allocate(pin);
                }

                this.timer.Start(TimeSpan.FromMilliseconds(10), this.settings.PollInterval);
                this.IsOpened = true;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            lock (this.timer)
            {
                if (!this.IsOpened)
                {
                    return;
                }

                this.timer.Stop();
                foreach (var pin in this.pinConfigurations.Values)
                {
                    this.Release(pin);
                }

                this.IsOpened = false;
            }
        }

        /// <summary>
        /// Clears pin attached to this connection.
        /// </summary>
        public void Clear()
        {
            lock (this.pinConfigurations)
            {
                foreach (var pinConfiguration in this.pinConfigurations.Values)
                {
                    this.Release(pinConfiguration);
                }

                this.pinConfigurations.Clear();
                this.namedPins.Clear();
                this.pinValues.Clear();

                this.pinRawValues = ProcessorPins.None;
                this.inputPins = ProcessorPins.None;
            }
        }

        /// <summary>
        /// Adds the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Add(PinConfiguration pin)
        {
            lock (this.pinConfigurations)
            {
                if (this.pinConfigurations.ContainsKey(pin.Pin))
                {
                    throw new InvalidOperationException("This pin is already present on the connection");
                }

                if (!string.IsNullOrEmpty(pin.Name) && this.namedPins.ContainsKey(pin.Name))
                {
                    throw new InvalidOperationException("A pin with the same name is already present on the connection");
                }

                this.pinConfigurations.Add(pin.Pin, pin);

                if (!string.IsNullOrEmpty(pin.Name))
                {
                    this.namedPins.Add(pin.Name, pin);
                }

                lock (this.timer)
                {
                    if (this.IsOpened)
                    {
                        this.Allocate(pin);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string pinName)
        {
            return this.namedPins.ContainsKey(pinName);
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ConnectorPin pin)
        {
            return this.pinConfigurations.ContainsKey(pin.ToProcessor());
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ProcessorPin pin)
        {
            return this.pinConfigurations.ContainsKey(pin);
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PinConfiguration configuration)
        {
            return this.pinConfigurations.ContainsKey(configuration.Pin);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        public void Remove(string pinName)
        {
            this.Remove(this.namedPins[pinName]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Remove(ConnectorPin pin)
        {
            this.Remove(this.pinConfigurations[pin.ToProcessor()]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Remove(ProcessorPin pin)
        {
            this.Remove(this.pinConfigurations[pin]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        public void Remove(PinConfiguration configuration)
        {
            lock (this.pinConfigurations)
            {
                lock (this.timer)
                {
                    if (this.IsOpened)
                    {
                        this.Release(configuration);
                    }
                }

                this.pinConfigurations.Remove(configuration.Pin);
                if (!string.IsNullOrEmpty(configuration.Name))
                {
                    this.namedPins.Remove(configuration.Name);
                }

                this.pinValues.Remove(configuration.Pin);

                var pin = (ProcessorPins)(1U << (int)configuration.Pin);
                this.inputPins = this.inputPins & ~pin;
                this.pinRawValues = this.pinRawValues & ~pin;
            }
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        public void Toggle(string pinName)
        {
            this[pinName] = !this[pinName];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toggle(ProcessorPin pin)
        {
            this[pin] = !this[pin];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toggle(ConnectorPin pin)
        {
            this[pin] = !this[pin];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        public void Toggle(PinConfiguration configuration)
        {
            this[configuration] = !this[configuration];
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(string pinName, TimeSpan duration = default)
        {
            this.Toggle(pinName);
            this.Sleep(duration);
            this.Toggle(pinName);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(ProcessorPin pin, TimeSpan duration = default)
        {
            this.Toggle(pin);
            this.Sleep(duration);
            this.Toggle(pin);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(ConnectorPin pin, TimeSpan duration = default)
        {
            this.Toggle(pin);
            this.Sleep(duration);
            this.Toggle(pin);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(PinConfiguration configuration, TimeSpan duration = default)
        {
            this.Toggle(configuration);
            this.Sleep(duration);
            this.Toggle(configuration);
        }

        internal PinConfiguration GetConfiguration(string pinName)
        {
            return this.namedPins[pinName];
        }

        internal PinConfiguration GetConfiguration(ProcessorPin pin)
        {
            return this.pinConfigurations[pin];
        }

        /// <summary>
        /// Raises the <see cref="PinStatusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Pi.IO.GeneralPurpose.PinStatusEventArgs"/> instance containing the event data.</param>
        protected void OnPinStatusChanged(PinStatusEventArgs e)
        {
            this.PinStatusChanged?.Invoke(this, e);
        }

        private void Sleep(TimeSpan duration)
        {
            this.thread.Sleep(duration <= TimeSpan.Zero ? this.settings.BlinkDuration : duration);
        }

        private void Allocate(PinConfiguration configuration)
        {
            if (configuration.StatusChangedAction != null)
            {
                var handler = new EventHandler<PinStatusEventArgs>((sender, args) =>
                                                                       {
                                                                           if (args.Configuration == configuration)
                                                                           {
                                                                               configuration.StatusChangedAction(args.Enabled);
                                                                           }
                                                                       });
                this.pinEvents[configuration.Pin] = handler;
                this.PinStatusChanged += handler;
            }

            this.gpioConnectionDriver.Allocate(configuration.Pin, configuration.Direction);
            if (configuration is OutputPinConfiguration outputConfiguration)
            {
                this[configuration.Pin] = outputConfiguration.Enabled;
            }
            else
            {
                var inputConfiguration = (InputPinConfiguration)configuration;
                var pinValue = this.gpioConnectionDriver.Read(inputConfiguration.Pin);

                var pin = (ProcessorPins)(1U << (int)inputConfiguration.Pin);
                this.inputPins = this.inputPins | pin;
                this.pinRawValues = this.gpioConnectionDriver.Read(this.inputPins);

                if (inputConfiguration.Resistor != PinResistor.None &&
                    (this.gpioConnectionDriver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) > 0)
                {
                    this.gpioConnectionDriver.SetPinResistor(inputConfiguration.Pin, inputConfiguration.Resistor);
                }

                if (inputConfiguration is SwitchInputPinConfiguration switchConfiguration)
                {
                    this.pinValues[inputConfiguration.Pin] = switchConfiguration.Enabled;
                    this.OnPinStatusChanged(new PinStatusEventArgs { Configuration = inputConfiguration, Enabled = this.pinValues[inputConfiguration.Pin] });
                }
                else
                {
                    this.pinValues[inputConfiguration.Pin] = inputConfiguration.GetEffective(pinValue);
                    this.OnPinStatusChanged(new PinStatusEventArgs { Configuration = inputConfiguration, Enabled = this.pinValues[inputConfiguration.Pin] });
                }
            }
        }

        private void Release(PinConfiguration configuration)
        {
            if (configuration.Direction == PinDirection.Output)
            {
                this.gpioConnectionDriver.Write(configuration.Pin, false);
                this.OnPinStatusChanged(new PinStatusEventArgs { Enabled = false, Configuration = configuration });
            }

            this.gpioConnectionDriver.Release(configuration.Pin);

            if (this.pinEvents.TryGetValue(configuration.Pin, out var handler))
            {
                this.PinStatusChanged -= handler;
                this.pinEvents.Remove(configuration.Pin);
            }
        }

        private void CheckInputPins(ITimer timer)
        {
            var newPinValues = this.gpioConnectionDriver.Read(this.inputPins);

            var changes = newPinValues ^ this.pinRawValues;
            if (changes == ProcessorPins.None)
            {
                return;
            }

            var notifiedConfigurations = new List<PinConfiguration>();
            foreach (var np in changes.Enumerate())
            {
                var processorPin = (ProcessorPins)(1U << (int)np);
                var oldPinValue = (this.pinRawValues & processorPin) != ProcessorPins.None;
                var newPinValue = (newPinValues & processorPin) != ProcessorPins.None;

                if (oldPinValue != newPinValue)
                {
                    var pin = (InputPinConfiguration)this.pinConfigurations[np];
                    if (pin is SwitchInputPinConfiguration)
                    {
                        if (pin.GetEffective(newPinValue))
                        {
                            this.pinValues[np] = !this.pinValues[np];
                            notifiedConfigurations.Add(pin);
                        }
                    }
                    else
                    {
                        this.pinValues[np] = pin.GetEffective(newPinValue);
                        notifiedConfigurations.Add(pin);
                    }
                }
            }

            this.pinRawValues = newPinValues;

            // Only fires events once all states have been modified.
            foreach (var pin in notifiedConfigurations)
            {
                this.OnPinStatusChanged(new PinStatusEventArgs { Configuration = pin, Enabled = this.pinValues[pin.Pin] });
            }
        }
    }
}