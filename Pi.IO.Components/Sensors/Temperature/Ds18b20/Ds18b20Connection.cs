// <copyright file="Ds18b20Connection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Temperature.Ds18b20
{
    using global::System;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Threading;

    /// <summary>
    /// Represents a connection to a DS18B20 temperature sensor.
    /// </summary>
    /// <remarks>See <see href="https://learn.adafruit.com/adafruits-raspberry-pi-lesson-11-ds18b20-temperature-sensing"/>.</remarks>
    public class Ds18B20Connection : IDisposable
    {
        private const string BaseDir = @"/sys/bus/w1/devices/";

        private readonly string deviceFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18B20Connection" /> class.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <exception cref="ArgumentException">Thrown if deviceFolder does not exists.</exception>
        public Ds18B20Connection(string deviceId)
        {
            this.deviceFolder = BaseDir + deviceId;
            if (!Directory.Exists(this.deviceFolder))
            {
                throw new ArgumentException(string.Format("Sensor with Id {0} not found. {1}", deviceId, this.ModprobeExceptionMessage), deviceId);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18B20Connection" /> class.
        /// </summary>
        /// <param name="deviceIndex">Index of the device.</param>
        /// <exception cref="InvalidOperationException">Thrown if deviceCount is 0.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if deviceCount is 0 or less than deviceIndex.</exception>
        public Ds18B20Connection(int deviceIndex)
        {
            var deviceFolders = Directory.GetDirectories(BaseDir, "28*").ToList();
            var deviceCount = deviceFolders.Count();
            if (deviceCount == 0)
            {
                throw new InvalidOperationException(string.Format("No sensors were found in {0}. {1}", BaseDir, this.ModprobeExceptionMessage));
            }

            if (deviceCount <= deviceIndex)
            {
                throw new IndexOutOfRangeException(string.Format("There were only {0} devices found, so index {1} is invalid", deviceCount, deviceIndex));
            }

            this.deviceFolder = deviceFolders[deviceIndex];
        }

        private string DeviceFile => this.deviceFolder + @"/w1_slave";

        private string ModprobeExceptionMessage
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Make sure you have loaded the required kernel models.");
                sb.AppendFormat("\tmodprobe w1-gpio{0}", Environment.NewLine);
                sb.AppendFormat("\tmodprobe w1-therm{0}", Environment.NewLine);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        /// <returns>The temperature.</returns>
        public UnitsNet.Temperature GetTemperature()
        {
            var lines = File.ReadAllLines(this.DeviceFile);
            while (!lines[0].Trim().EndsWith("YES"))
            {
                Thread.Sleep(2);
                lines = File.ReadAllLines(this.DeviceFile);
            }

            var equalsPos = lines[1].IndexOf("t=", StringComparison.InvariantCultureIgnoreCase);
            if (equalsPos == -1)
            {
                throw new InvalidOperationException("Unable to read temperature");
            }

            var temperatureString = lines[1].Substring(equalsPos + 2);

            return UnitsNet.Temperature.FromDegreesCelsius(double.Parse(temperatureString, CultureInfo.InvariantCulture) / 1000.0);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
        }
    }
}
