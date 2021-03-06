// <copyright file="Board.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Pi.Core.Threading;

    /// <summary>
    /// Represents the Raspberry Pi mainboard.
    /// </summary>
    /// <remarks>
    /// Version and revisions are based on <see href="http://raspberryalphaomega.org.uk/2013/02/06/automatic-raspberry-pi-board-revision-detection-model-a-b1-and-b2/"/>.
    /// <see href="http://www.raspberrypi-spy.co.uk/2012/09/checking-your-raspberry-pi-board-version/"/> for information.
    /// </remarks>
    public class Board
    {
        private static readonly Lazy<Board> BoardLazy = new Lazy<Board>(LoadBoard);
        private readonly Dictionary<string, string> settings;
        private readonly Lazy<Model> model;
        private readonly Lazy<string> processorName;
        private readonly Lazy<Processor> processor;
        private readonly Lazy<ConnectorPinout> connectorPinout;
        private readonly Lazy<IThreadFactory> threadFactory;

        private Board(Dictionary<string, string> settings)
        {
            this.model = new Lazy<Model>(this.LoadModel);
            this.processorName = new Lazy<string>(() =>
                this.settings.TryGetValue("Hardware", out var hardware) ? hardware : null);
            this.processor = new Lazy<Processor>(() =>
                Enum.TryParse(this.ProcessorName, true, out Processor processor) ? processor : Processor.Unknown);
            this.connectorPinout = new Lazy<ConnectorPinout>(this.LoadConnectorPinout);
            this.threadFactory = new Lazy<IThreadFactory>(() => new ThreadFactory(this, true));
            this.settings = settings;
        }

        /// <summary>
        /// Gets the current mainboard configuration.
        /// </summary>
        public static Board Current => BoardLazy.Value;

        /// <summary>
        /// Gets the thread factory.
        /// </summary>
        /// <value>
        /// The thread factory.
        /// </value>
        public IThreadFactory ThreadFactory => this.threadFactory.Value;

        /// <summary>
        /// Gets a value indicating whether this instance is a Raspberry Pi.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is a Raspberry Pi; otherwise, <c>false</c>.
        /// </value>
        public bool IsRaspberryPi => this.Processor != Processor.Unknown;

        /// <summary>
        /// Gets the processor name.
        /// </summary>
        /// <value>
        /// The name of the processor.
        /// </value>
        public string ProcessorName => this.processorName.Value;

        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>
        /// The processor.
        /// </value>
        public Processor Processor => this.processor.Value;

        /// <summary>
        /// Gets the board firmware version.
        /// </summary>
        public int Firmware
        {
            get
            {
                if (this.settings.TryGetValue("Revision", out var revision) &&
                    !string.IsNullOrEmpty(revision) &&
                    int.TryParse(revision, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var firmware))
                {
                    return firmware;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        public string SerialNumber
        {
            get
            {
                if (this.settings.TryGetValue("Serial", out var serial) && !string.IsNullOrEmpty(serial))
                {
                    return serial;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Raspberry Pi board is overclocked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Raspberry Pi is overclocked; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverclocked
        {
            get
            {
                var firmware = this.Firmware;
                return (firmware & 0xFFFF0000) != 0;
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public Model Model => this.model.Value;

        /// <summary>
        /// Gets the connector revision.
        /// </summary>
        /// <value>
        /// The connector revision.
        /// </value>
        /// <remarks>See <see href="http://raspi.tv/2014/rpi-gpio-quick-reference-updated-for-raspberry-pi-b"/> for more information.</remarks>
        public ConnectorPinout ConnectorPinout => this.connectorPinout.Value;

        private static Board LoadBoard()
        {
            try
            {
                const string filePath = "/proc/cpuinfo";

                var cpuInfo = File.ReadAllLines(filePath);
                var settings = new Dictionary<string, string>();
                var suffix = string.Empty;

                foreach (var l in cpuInfo)
                {
                    var separator = l.IndexOf(':');

                    if (!string.IsNullOrWhiteSpace(l) && separator > 0)
                    {
                        var key = l.Substring(0, separator).Trim();
                        var val = l.Substring(separator + 1).Trim();
                        if (string.Equals(key, "processor", StringComparison.InvariantCultureIgnoreCase))
                        {
                            suffix = "." + val;
                        }

                        settings.Add(key + suffix, val);
                    }
                    else
                    {
                        suffix = string.Empty;
                    }
                }

                return new Board(settings);
            }
            catch
            {
                return new Board(new Dictionary<string, string>());
            }
        }

        private Model LoadModel()
        {
            var firmware = this.Firmware;
            switch (firmware & 0xFFFF)
            {
                case 0x2:
                case 0x3:
                    return Model.BRev1;

                case 0x4:
                case 0x5:
                case 0x6:
                case 0xd:
                case 0xe:
                case 0xf:
                    return Model.BRev2;

                case 0x7:
                case 0x8:
                case 0x9:
                    return Model.A;

                case 0x10:
                    return Model.BPlus;

                case 0x11:
                    return Model.ComputeModule;

                case 0x12:
                    return Model.APlus;

                case 0x1040:
                case 0x1041:
                    return Model.B2;

                case 0x0092:
                case 0x0093:
                    return Model.Zero;

                case 0x2082:
                    return Model.B3;

                default:
                    return Model.Unknown;
            }
        }

        private ConnectorPinout LoadConnectorPinout()
        {
            switch (this.Model)
            {
                case Model.BRev1:
                    return ConnectorPinout.Rev1;

                case Model.BRev2:
                case Model.A:
                    return ConnectorPinout.Rev2;

                case Model.BPlus:
                case Model.ComputeModule:
                case Model.APlus:
                case Model.B2:
                case Model.Zero:
                case Model.B3:
                    return ConnectorPinout.Plus;

                default:
                    return ConnectorPinout.Unknown;
            }
        }
    }
}