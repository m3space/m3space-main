using System;
using System.IO.Ports;
using System.Text;

namespace NMEA
{
    #region Enums

    /// <summary>
    /// Перечисление скоростей порта
    /// </summary>
    public enum BaudRate : int
    {
        baudRate75 = 75,
        baudRate110 = 110,
        baudRate134 = 134,
        baudRate150 = 150,
        baudRate300 = 300,
        baudRate600 = 600,
        baudRate1200 = 1200,
        baudRate1800 = 1800,
        baudRate2400 = 2400,
        baudRate4800 = 4800,
        baudRate7200 = 7200,
        baudRate9600 = 9600,
        baudRate14400 = 14400,
        baudRate19200 = 19200,
        baudRate38400 = 38400,
        baudRate57600 = 57600,
        baudRate115200 = 115200,
        baudRate128000 = 128000
    }

    /// <summary>
    /// Перечисление для битов данных порта
    /// </summary>
    public enum DataBits
    {
        dataBits4 = 4,
        dataBits5 = 5,
        dataBits6 = 6,
        dataBits7 = 7,
        dataBits8 = 8
    }

    #endregion

    /// <summary>
    /// Класс-контейнер настроек последовательного порта
    /// </summary>
    [Serializable]
    public sealed class SerialPortSettings : ICloneable
    {
        #region Properties

        /// <summary>
        /// Имя порта
        /// </summary>
        public string PortName { get; set; }
        
        /// <summary>
        /// Скорость порта
        /// </summary>
        public BaudRate PortBaudRate { get; set; }
        
        /// <summary>
        /// Четность
        /// </summary>
        public Parity PortParity { get; set; }
        
        /// <summary>
        /// Биты данных
        /// </summary>
        public DataBits PortDataBits { get; set; }
        
        /// <summary>
        /// Стоповые биты
        /// </summary>
        public StopBits PortStopBits { get; set; }
       
        /// <summary>
        /// Handshake
        /// </summary>
        public Handshake PortHandshake { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public SerialPortSettings()
            : this("COM1")
        {
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="portNameParameter">Имя порта</param>
        public SerialPortSettings(string portNameParameter)
            : this(portNameParameter, BaudRate.baudRate9600, Parity.Even, DataBits.dataBits8, StopBits.One, Handshake.None)
        {
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="portNameParameter">Имя порта</param>
        /// <param name="portBaudRateParameter">Скорость</param>
        /// <param name="portParityParameter">Четность</param>
        /// <param name="portDataBitsParameter">Биты данных</param>
        /// <param name="portStopBitsParameter">Стоповые биты</param>
        /// <param name="portHandshakeParameter">Handshake</param>
        public SerialPortSettings(string portNameParameter,
            BaudRate portBaudRateParameter,
            Parity portParityParameter,
            DataBits portDataBitsParameter,
            StopBits portStopBitsParameter,
            Handshake portHandshakeParameter)
        {
            PortName = portNameParameter;
            PortBaudRate = portBaudRateParameter;
            PortParity = portParityParameter;
            PortDataBits = portDataBitsParameter;
            PortStopBits = portStopBitsParameter;
            PortHandshake = portHandshakeParameter;
        }

        #endregion

        #region Interfaces

        #region ICloneable Members

        /// <summary>
        /// Clone() IClonable member
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SerialPortSettings(PortName,
                PortBaudRate,
                PortParity,
                PortDataBits,
                PortStopBits,
                PortHandshake);
        }

        #endregion

        //#region IComparable<RS232Settings> Members

        //int IComparable<RS232Settings>.CompareTo(RS232Settings other)
        //{
        //    return this.ToString().CompareTo(other.ToString());
        //}

        //#endregion

        #endregion                 

        #region Methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(PortName);            
            sb.Append(" (");
            sb.Append(PortBaudRate.ToString());
            sb.Append(", ");
            sb.Append(PortParity.ToString());
            sb.Append(", ");
            sb.Append(PortDataBits.ToString());
            sb.Append(", ");
            sb.Append(PortStopBits.ToString());
            sb.Append(", ");
            sb.Append(PortHandshake.ToString());
            sb.Append(')');

            return sb.ToString();
        }

        //public override bool Equals(object obj)
        //{
        //    return ((obj is SerialPortSettings) && (StringComparer.Ordinal.Compare(this.ToString(), ((SerialPortSettings)(obj)).ToString()) == 0));
        //}

        //public override int GetHashCode()
        //{
        //    return this.ToString().GetHashCode();
        //}

        #endregion                
    }
}
