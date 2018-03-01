// <copyright file="BMP280.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IotSample
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using I2C.Net;

    public sealed class Bmp280
    {
        // String for the friendly name of the I2C bus
        private const string I2CBusPath = "/dev/i2c-1";
        private const byte Bmp280Address = 0x77;

        // t_fine carries fine temperature as global value
        private int tFine = int.MinValue;

        // Create new calibration data for the sensor
        private Bmp280CalibrationData calibrationData;

        // Variable to check if device is initialized
        private bool init = false;

        private I2CBus i2CBus;

        private enum ERegisters : byte
        {
            Bmp280RegisterDigT1 = 0x88,
            Bmp280RegisterDigT2 = 0x8A,
            Bmp280RegisterDigT3 = 0x8C,

            Bmp280RegisterDigP1 = 0x8E,
            Bmp280RegisterDigP2 = 0x90,
            Bmp280RegisterDigP3 = 0x92,
            Bmp280RegisterDigP4 = 0x94,
            Bmp280RegisterDigP5 = 0x96,
            Bmp280RegisterDigP6 = 0x98,
            Bmp280RegisterDigP7 = 0x9A,
            Bmp280RegisterDigP8 = 0x9C,
            Bmp280RegisterDigP9 = 0x9E,

            Bmp280RegisterChipid = 0xD0,
            Bmp280RegisterVersion = 0xD1,
            Bmp280RegisterSoftreset = 0xE0,

            Bmp280RegisterCal26 = 0xE1,  // R calibration stored in 0xE1-0xF0

            Bmp280RegisterControlhumid = 0xF2,
            Bmp280RegisterControl = 0xF4,
            Bmp280RegisterConfig = 0xF5,

            Bmp280RegisterPressuredataMsb = 0xF7,
            Bmp280RegisterPressuredataLsb = 0xF8,
            Bmp280RegisterPressuredataXlsb = 0xF9, // bits <7:4>

            Bmp280RegisterTempdataMsb = 0xFA,
            Bmp280RegisterTempdataLsb = 0xFB,
            Bmp280RegisterTempdataXlsb = 0xFC, // bits <7:4>

            Bmp280RegisterHumiddataMsb = 0xFD,
            Bmp280RegisterHumiddataLsb = 0xFE,
        }

        // Method to initialize the BMP280 sensor
        public void Initialize()
        {
            Debug.WriteLine("BMP280::Initialize");

            try
            {
                // read from I2C device bus 1
                this.i2CBus = I2CBus.Open(I2CBusPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        public Task<float> ReadTemperature()
        {
            return Task.Run(async () => (float)await this.ReadTemperatureAsync());
        }

        public Task<float> ReadPreasure()
        {
            return Task.Run(async () => (float)await this.ReadPreasureAsync());
        }

        private async Task Begin()
        {
            Console.WriteLine("BMP280::Begin");

            // Read the device signature
            this.i2CBus.WriteCommand(Bmp280Address, (byte)ERegisters.Bmp280RegisterChipid);
            var buffer = this.i2CBus.ReadBytes(Bmp280Address, 1);
            Console.WriteLine("BMP280 Signature: " + buffer[0].ToString());

            // Set the initalize variable to true
            this.init = true;

            // Read the coefficients table
            this.calibrationData = await this.ReadCoefficeints();

            // Write control register
            await this.WriteControlRegister();

            // Write humidity control register
            await this.WriteControlRegisterHumidity();
        }

        // Method to write 0x03 to the humidity control register
        private async Task WriteControlRegisterHumidity()
        {
            this.i2CBus.WriteBytes(Bmp280Address, new byte[] { (byte)ERegisters.Bmp280RegisterControlhumid, 0x03 });
            await Task.Delay(1);
            return;
        }

        // Method to write 0x3F to the control register
        private async Task WriteControlRegister()
        {
            this.i2CBus.WriteBytes(Bmp280Address, new byte[] { (byte)ERegisters.Bmp280RegisterControl, 0x3F });
            await Task.Delay(1);
            return;
        }

        // Method to read a 16-bit value from a register and return it in little endian format
        private ushort ReadUInt16_LittleEndian(byte register)
        {
            this.i2CBus.WriteCommand(Bmp280Address, register);
            var buffer = this.i2CBus.ReadBytes(Bmp280Address, 2);

            int h = buffer[1] << 8;
            int l = buffer[0];
            return (ushort)(h + l);
        }

        // Method to read an 8-bit value from a register
        private byte ReadByte(byte register)
        {
            this.i2CBus.WriteCommand(Bmp280Address, register);
            var readBuffer = this.i2CBus.ReadBytes(Bmp280Address, 2);
            return readBuffer[0];
        }

        // Method to read the caliberation data from the registers
        private async Task<Bmp280CalibrationData> ReadCoefficeints()
        {
            // 16 bit calibration data is stored as Little Endian, the helper method will do the byte swap.
            this.calibrationData = new Bmp280CalibrationData();

            // Read temperature calibration data
            this.calibrationData.DigT1 = this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigT1);
            this.calibrationData.DigT2 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigT2);
            this.calibrationData.DigT3 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigT3);

            // Read presure calibration data
            this.calibrationData.DigP1 = this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP1);
            this.calibrationData.DigP2 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP2);
            this.calibrationData.DigP3 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP3);
            this.calibrationData.DigP4 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP4);
            this.calibrationData.DigP5 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP5);
            this.calibrationData.DigP6 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP6);
            this.calibrationData.DigP7 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP7);
            this.calibrationData.DigP8 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP8);
            this.calibrationData.DigP9 = (short)this.ReadUInt16_LittleEndian((byte)ERegisters.Bmp280RegisterDigP9);

            await Task.Delay(1);
            return this.calibrationData;
        }

        // Method to return the temperature in DegC. Resolution is 0.01 DegC. Output value of “5123” equals 51.23 DegC.
        private double BMP280_compensate_T_double(int adcT)
        {
            double var1, var2, temperature;

            // The temperature is calculated using the compensation formula in the BMP280 datasheet
            var1 = ((adcT / 16384.0) - (this.calibrationData.DigT1 / 1024.0)) * this.calibrationData.DigT2;
            var2 = ((adcT / 131072.0) - (this.calibrationData.DigT1 / 8192.0)) * this.calibrationData.DigT3;

            this.tFine = (int)(var1 + var2);

            temperature = (var1 + var2) / 5120.0;
            return temperature;
        }

        // Method to returns the pressure in Pa, in Q24.8 format (24 integer bits and 8 fractional bits).
        // Output value of “24674867” represents 24674867/256 = 96386.2 Pa = 963.862 hPa
        private long BMP280_compensate_P_Int64(int adcP)
        {
            long var1, var2, p;

            // The pressure is calculated using the compensation formula in the BMP280 datasheet
            var1 = this.tFine - 128000;
            var2 = var1 * var1 * (long)this.calibrationData.DigP6;
            var2 = var2 + ((var1 * (long)this.calibrationData.DigP5) << 17);
            var2 = var2 + ((long)this.calibrationData.DigP4 << 35);
            var1 = ((var1 * var1 * (long)this.calibrationData.DigP3) >> 8) + ((var1 * (long)this.calibrationData.DigP2) << 12);
            var1 = ((((long)1 << 47) + var1) * (long)this.calibrationData.DigP1) >> 33;
            if (var1 == 0)
            {
                Debug.WriteLine("BMP280_compensate_P_Int64 Jump out to avoid / 0");
                return 0; // Avoid exception caused by division by zero
            }

            // Perform calibration operations as per datasheet: http://www.adafruit.com/datasheets/BST-BMP280-DS001-11.pdf
            p = 1048576 - adcP;
            p = ((p << 31) - var2) * 3125 / var1;
            var1 = ((long)this.calibrationData.DigP9 * (p >> 13) * (p >> 13)) >> 25;
            var2 = ((long)this.calibrationData.DigP8 * p) >> 19;
            p = ((p + var1 + var2) >> 8) + ((long)this.calibrationData.DigP7 << 4);
            return p;
        }

        private async Task<float> ReadTemperatureAsync()
        {
            // Make sure the I2C device is initialized
            if (!this.init)
            {
                await this.Begin();
            }

            // Read the MSB, LSB and bits 7:4 (XLSB) of the temperature from the BMP280 registers
            byte tmsb = this.ReadByte((byte)ERegisters.Bmp280RegisterTempdataMsb);
            byte tlsb = this.ReadByte((byte)ERegisters.Bmp280RegisterTempdataLsb);
            byte txlsb = this.ReadByte((byte)ERegisters.Bmp280RegisterTempdataXlsb); // bits 7:4

            // Combine the values into a 32-bit integer
            int t = (tmsb << 12) + (tlsb << 4) + (txlsb >> 4);

            // Convert the raw value to the temperature in degC
            double temp = this.BMP280_compensate_T_double(t);

            // Return the temperature as a float value
            return (float)temp;
            }

        private async Task<float> ReadPreasureAsync()
        {
            // Make sure the I2C device is initialized
            if (!this.init)
            {
                await this.Begin();
            }

            // Read the temperature first to load the t_fine value for compensation
            if (this.tFine == int.MinValue)
            {
                await this.ReadTemperature();
            }

            // Read the MSB, LSB and bits 7:4 (XLSB) of the pressure from the BMP280 registers
            byte tmsb = this.ReadByte((byte)ERegisters.Bmp280RegisterPressuredataMsb);
            byte tlsb = this.ReadByte((byte)ERegisters.Bmp280RegisterPressuredataLsb);
            byte txlsb = this.ReadByte((byte)ERegisters.Bmp280RegisterPressuredataXlsb); // bits 7:4

            // Combine the values into a 32-bit integer
            int t = (tmsb << 12) + (tlsb << 4) + (txlsb >> 4);

            // Convert the raw value to the pressure in Pa
            long pres = this.BMP280_compensate_P_Int64(t);

            // Return the pressure as a float value
            return (float)pres / 256;
        }
    }
}
