using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace BalloonFirmware.Drivers
{
    /// <summary>
    /// Abstract base class for I2C devices.
    /// </summary>
    public abstract class I2CSlave
    {
        protected const int ClockRate = 100;
        protected int Timeout = 1000;

        protected I2CDevice i2c;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="address">the device address</param>
        protected I2CSlave(ushort address)
        {
            i2c = new I2CDevice(new I2CDevice.Configuration(address, ClockRate));
        }

        /// <summary>
        /// Writes a sequences of bytes to the I2C bus.
        /// The first byte is the starting register, the second byte the register value.
        /// All following bytes are values of subsequent registers.
        /// </summary>
        /// <param name="writeBuffer">the data to write</param>
        /// <returns>true if successful, false if failed</returns>
        protected bool WriteBytes(byte[] writeBuffer)
        {
            I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[] {
                I2CDevice.CreateWriteTransaction(writeBuffer)
            };

            int written = i2c.Execute(writeTransaction, Timeout);

            while (written < writeBuffer.Length)
            {
                byte[] newBuffer = new byte[writeBuffer.Length - written];
                Array.Copy(writeBuffer, written, newBuffer, 0, newBuffer.Length);

                writeTransaction = new I2CDevice.I2CTransaction[] {
                    I2CDevice.CreateWriteTransaction(newBuffer)
                };

                written += i2c.Execute(writeTransaction, Timeout);
            }

            return (written == writeBuffer.Length);
        }

        /// <summary>
        /// Reads a sequence of bytes from subsequent registers.
        /// The starting register has to be written first.
        /// </summary>
        /// <param name="readBuffer">the buffer to read into</param>
        /// <returns>true if successful, false if failed</returns>
        protected bool ReadBytes(byte[] readBuffer)
        {
            I2CDevice.I2CTransaction[] readTransaction = new I2CDevice.I2CTransaction[] {
                I2CDevice.CreateReadTransaction(readBuffer)
            };

            int read = i2c.Execute(readTransaction, Timeout);

            return (read == readBuffer.Length);
        }

        /// <summary>
        /// Writes a single register value.
        /// </summary>
        /// <param name="register">the register address</param>
        /// <param name="value">the value</param>
        /// <returns>true if successful, false if failed</returns>
        protected bool WriteToRegister(byte register, byte value)
        {
            return WriteBytes(new byte[] { register, value });
        }

        /// <summary>
        /// Reads a sequence of bytes from subsequent registers.
        /// </summary>
        /// <param name="register">the starting register</param>
        /// <param name="readBuffer">the buffer to read into</param>
        /// <returns>true if successful, false if failed</returns>
        protected bool ReadFromRegister(byte register, byte[] readBuffer)
        {
            if (WriteBytes(new byte[] { register }))
            {
                return ReadBytes(readBuffer);
            }
            return false;
        }

        /// <summary>
        /// Overwrites some bits inside a register.
        /// </summary>
        /// <param name="register">the register address</param>
        /// <param name="startPos">the start bit position (7=MSB, 0=LSB)</param>
        /// <param name="length">the number of bits to write</param>
        /// <param name="rValue">the right-aligned value to write</param>
        /// <returns>true if successful, false if failed</returns>
        protected bool WriteBitsToRegister(byte register, byte startPos, byte length, byte rValue)
        {
            byte[] bValue = new byte[1];
            if (ReadFromRegister(register, bValue))
            {
                byte mask = (byte)(((1 << length) - 1) << (startPos - length + 1));
                rValue <<= (startPos - length + 1);
                rValue &= mask;
                bValue[0] &= (byte)~(mask);
                bValue[0] |= rValue;
                return WriteToRegister(register, bValue[0]);
            }
            return false;
        }
    }
}
