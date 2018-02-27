using Pi.IO.InterIntegratedCircuit;
using Pi.Timers;
using Pi.System.Threading;

namespace Pi.IO.Components.Sensors.Light
{
    namespace RPI.Sensor.Sensors.Light
    {
        public class BH1750Connection
        {
            private readonly IThread thread;

            public BH1750Connection(I2cDeviceConnection connection, IThread thread)
            {
                this.thread = thread;
                this.Connection = connection;
            }

            public I2cDeviceConnection Connection { get; set; }
            
            public void SetOff()
            {
                this.Connection.Write(0x00);
            }

            public void SetOn()
            {
                this.Connection.Write(0x01);
            }

            public void Reset()
            {
                this.Connection.Write(0x07);
            }

            public double GetData()
            {
                this.Connection.Write(0x10);
                this.thread.Sleep(TimeSpanUtility.FromMicroseconds(150 * 1000));
                byte[] readBuf = this.Connection.Read(2);

                var valf = readBuf[0] << 8;
                valf |= readBuf[1];
                return valf / 1.2 * (69 / 69) / 1;

                // var valf = ((readBuf[0] << 8) | readBuf[1]) / 1.2;
                // return valf;

                // return Math.Round(valf / (2 * 1.2), 2);

            }
        }
    }
}
