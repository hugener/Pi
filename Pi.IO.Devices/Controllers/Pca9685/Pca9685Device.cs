// <copyright file="Pca9685Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Pca9685
{
    using global::Pi.IO.InterIntegratedCircuit;
    using global::Pi.System.Threading;
    using global::System;
    using global::UnitsNet;

    /// <summary>
    /// Driver for Adafruit 16-channel PWM/Servo Shield which uses the
    /// NXP PCA9685 16-channel, 12-bit PWM Fm+ I2C-bus LED controller
    /// Ported from
    /// https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Adafruit_PWM_Servo_Driver.py
    /// </summary>
    public class Pca9685Device : IPwmDevice
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(5);

        private readonly I2cDeviceConnection connection;
        private readonly IPca9685DeviceReporter pca9685DeviceReporter;
        private readonly IThread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pca9685Device" /> class.
        /// </summary>
        /// <param name="connection">The I2C connection.</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <param name="pca9685DeviceReporter">The pca9685 device reporter.</param>
        public Pca9685Device(I2cDeviceConnection connection, IThreadFactory threadFactory = null, IPca9685DeviceReporter pca9685DeviceReporter = null)
        {
            this.connection = connection;
            this.pca9685DeviceReporter = pca9685DeviceReporter;
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();

            this.pca9685DeviceReporter?.Resetting();
            this.WriteRegister(Register.Mode1, 0x00);
        }

        private enum Register
        {
            // SUBADR1 = 0x02,
            // SUBADR2 = 0x03,
            // SUBADR3 = 0x04,
            Mode1 = 0x00,
            Prescale = 0xFE,
            Led0OnL = 0x06,
            Led0OnH = 0x07,
            Led0OffL = 0x08,
            Led0OffH = 0x09,

            // ALLLED_ON_L = 0xFA,
            // ALLLED_ON_H = 0xFB,
            // ALLLED_OFF_L = 0xFC,
            // ALLLED_OFF_H = 0xFD,
        }

        /// <summary>
        /// Sets the PWM update rate.
        /// </summary>
        /// <param name="frequency">The frequency, in hz.</param>
        /// <remarks>Datasheet: 7.3.5 PWM frequency PRE_SCALE</remarks>
        public void SetPwmUpdateRate(Frequency frequency)
        {
            var preScale = 25000000.0m; // 25MHz
            preScale /= 4096m; // 12-bit
            preScale /= (int)frequency.Hertz;

            preScale -= 1.0m;

            this.pca9685DeviceReporter?.SettingFrequency(frequency);
            this.pca9685DeviceReporter?.EstimatedPremaximum(preScale);

            var prescale = Math.Floor(preScale + 0.5m);

            this.pca9685DeviceReporter?.FinalPremaximum(preScale);

            var oldmode = this.ReadRegister(Register.Mode1);
            var newmode = (byte)((oldmode & 0x7F) | 0x10); // sleep

            this.WriteRegister(Register.Mode1, newmode); // go to sleep

            this.WriteRegister(Register.Prescale, (byte)Math.Floor(prescale));
            this.WriteRegister(Register.Mode1, oldmode);

            this.thread.Sleep(Delay);

            this.WriteRegister(Register.Mode1, oldmode | 0x80);
        }

        /// <summary>
        /// Sets a single PWM channel
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="on">The on values.</param>
        /// <param name="off">The off values.</param>
        public void SetPwm(PwmChannel channel, int on, int off)
        {
            this.WriteRegister(Register.Led0OnL + (4 * (int)channel), on & 0xFF);
            this.WriteRegister(Register.Led0OnH + (4 * (int)channel), on >> 8);
            this.WriteRegister(Register.Led0OffL + (4 * (int)channel), off & 0xFF);
            this.WriteRegister(Register.Led0OffH + (4 * (int)channel), off >> 8);
        }

        /// <summary>
        /// Set a channel to fully on or off
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="fullOn">if set to <c>true</c>, all values are on; otherwise they are all off.</param>
        public void SetFull(PwmChannel channel, bool fullOn)
        {
            if (fullOn)
            {
                this.SetFullOn(channel);
            }
            else
            {
                this.SetFullOff(channel);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.thread.Dispose();
        }

        private void SetFullOn(PwmChannel channel)
        {
            this.WriteRegister(Register.Led0OnH + (4 * (int)channel), 0x10);
            this.WriteRegister(Register.Led0OffH + (4 * (int)channel), 0x00);
        }

        private void SetFullOff(PwmChannel channel)
        {
            this.WriteRegister(Register.Led0OnH + (4 * (int)channel), 0x00);
            this.WriteRegister(Register.Led0OffH + (4 * (int)channel), 0x10);
        }

        private void WriteRegister(Register register, byte data)
        {
            this.connection.Write(new[] { (byte)register, data });
        }

        private void WriteRegister(Register register, int data)
        {
            this.WriteRegister(register, (byte)data);
        }

        private byte ReadRegister(Register register)
        {
            this.connection.Write((byte)register);
            var value = this.connection.ReadByte();
            return value;
        }
    }
}
