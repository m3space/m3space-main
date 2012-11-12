using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace BalloonFirmware.Drivers
{
    /// <summary>
    /// MPU6050 Gyro/Accelerometer implementation.
    /// </summary>
    public class Mpu6050 : I2CSlave
    {
        const byte MPU6050_ADDRESS_AD0_LOW = 0x68;      // device address
        const byte MPU6050_ADDRESS_AD0_HIGH = 0x69;

        const byte MPU6050_RA_GYRO_CONFIG = 0x1B;
        const byte MPU6050_RA_ACCEL_CONFIG = 0x1C;
        const byte MPU6050_RA_ACCEL_XOUT_H = 0x3B;      // first register of motion data
        const byte MPU6050_RA_PWR_MGMT_1 = 0x6B;

        const byte MPU6050_PWR1_CLKSEL_BIT = 2;
        const byte MPU6050_PWR1_CLKSEL_LENGTH = 3;
        const byte MPU6050_CLOCK_PLL_XGYRO = 0x01;

        const byte MPU6050_GCONFIG_FS_SEL_BIT = 4;
        const byte MPU6050_GCONFIG_FS_SEL_LENGTH = 2;

        const byte MPU6050_ACONFIG_AFS_SEL_BIT = 4;
        const byte MPU6050_ACONFIG_AFS_SEL_LENGTH = 2;

        const byte MPU6050_PWR1_SLEEP_BIT = 6;
        const byte MPU6050_PWR1_SLEEP_LENGTH = 1;

        const byte MPU6050_GYRO_FS_250 = 0x00;
        const byte MPU6050_GYRO_FS_500 = 0x01;
        const byte MPU6050_GYRO_FS_1000 = 0x02;
        const byte MPU6050_GYRO_FS_2000 = 0x03;

        const byte MPU6050_ACCEL_FS_2 = 0x00;
        const byte MPU6050_ACCEL_FS_4 = 0x01;
        const byte MPU6050_ACCEL_FS_8 = 0x02;
        const byte MPU6050_ACCEL_FS_16 = 0x03;


        private byte[] motionBuffer;        

        /// <summary>
        /// Constructor.
        /// </summary>
        public Mpu6050() : base(MPU6050_ADDRESS_AD0_LOW)
        {
            motionBuffer = new byte[14];
        }
        
        /// <summary>
        /// Sets up the device.
        /// </summary>
        public void Initialize()
        {
            // set clock source to X-gyro reference
            WriteBitsToRegister(MPU6050_RA_PWR_MGMT_1, MPU6050_PWR1_CLKSEL_BIT, MPU6050_PWR1_CLKSEL_LENGTH, MPU6050_CLOCK_PLL_XGYRO);
            // set full-scale gyro range
            WriteBitsToRegister(MPU6050_RA_GYRO_CONFIG, MPU6050_GCONFIG_FS_SEL_BIT, MPU6050_GCONFIG_FS_SEL_LENGTH, MPU6050_GYRO_FS_1000);
            // set full-scale accelerometer range
            WriteBitsToRegister(MPU6050_RA_ACCEL_CONFIG, MPU6050_ACONFIG_AFS_SEL_BIT, MPU6050_ACONFIG_AFS_SEL_LENGTH, MPU6050_ACCEL_FS_16);
            // disable sleep
            WriteBitsToRegister(MPU6050_RA_PWR_MGMT_1, MPU6050_PWR1_SLEEP_BIT, MPU6050_PWR1_SLEEP_LENGTH, 0);
        }

        /// <summary>
        /// Reads 6-axis motion data.
        /// </summary>
        /// <param name="data">the motion data to write to</param>
        /// <returns>true if successful, false if failed</returns>
        public bool GetMotionData(MotionData data)
        {
            if (ReadFromRegister(MPU6050_RA_ACCEL_XOUT_H, motionBuffer))
            {
                data.UtcTimestamp = DateTime.Now;
                data.Ax = BitConverter.ToInt16BigEndian(motionBuffer, 0);
                data.Ay = BitConverter.ToInt16BigEndian(motionBuffer, 2);
                data.Az = BitConverter.ToInt16BigEndian(motionBuffer, 4);
                data.Gx = BitConverter.ToInt16BigEndian(motionBuffer, 8);
                data.Gy = BitConverter.ToInt16BigEndian(motionBuffer, 10);
                data.Gz = BitConverter.ToInt16BigEndian(motionBuffer, 12);
                return true;
            }
            return false;
        }
    }
}
