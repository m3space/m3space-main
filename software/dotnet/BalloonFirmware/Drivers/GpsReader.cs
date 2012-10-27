using System;

namespace BalloonFirmware.Drivers
{
    public class GpsReader
    {
        private GpsPoint m_gpsPoint = new GpsPoint();

        public delegate void GpsDataProcessor(GpsPoint gpsPoint);
        public event GpsDataProcessor GpsDataReceived;


        public GpsReader()
        {
        }

        public string GetNmeaString(string strNMEA)
        {
            strNMEA = strNMEA.Trim("\r\n".ToCharArray()); //Remove linefeeds

            int nStart = strNMEA.IndexOf("$"); //Position of first NMEA data
            if (nStart < 0 || nStart == strNMEA.Length - 2)
                return strNMEA;

            //This will never pass the last NMEA sentence, before the next one arrives
            //The following should instead stop at the end of the line. 
            int nStop = strNMEA.IndexOf("$", nStart + 1); //Position of next NMEA sentence

            if (nStop > -1)
            {
                string strData;
                strData = strNMEA.Substring(nStart, nStop - nStart).Trim();

                if (CheckSentence(strData))
                {
                    ParseNMEA(strData);
                }

                return strNMEA.Substring(nStop);
            }
            else
                return strNMEA;
        }

        private void OnGpsData(GpsPoint gpsPoint)
        {
            if (GpsDataReceived != null)
                GpsDataReceived(gpsPoint);
        }

        private bool CheckSentence(string strSentence)
        {
            int iStart = strSentence.IndexOf('$');
            int iEnd = strSentence.IndexOf('*');
            if (iStart != 0 || iEnd < 0 || strSentence.Length != iEnd + 3)
                return false;

            // validate checksum
            byte result = 0;
            for (int i = iStart + 1; i < iEnd; i++)
                result ^= (byte)strSentence[i];

            bool checksumOK = (result == Convert.ToInt32(strSentence.Substring(iEnd + 1, 2), 16));
            if (!checksumOK)
            {

            }
            return checksumOK;
        }

        private void ParseNMEA(string line)
        {
            if (line.IndexOf("$GPGGA") == 0)
            {
                // Parse GGA - Global Positioning System Fix Data, Time, Position and fix related data fora GPS receiver. 
                // $GPGGA,191410,4735.5634,N,00739.3538,E,1,04,4.4,351.5,M,48.0,M,,*45
                try
                {
                    string[] parts = line.Split(',');
                    if (parts.Length != 15 || parts[6] == "0")
                        return;

                    m_gpsPoint.Satellites = Byte.Parse(parts[7]); //satelites
                    m_gpsPoint.Altitude = (ushort)Double.Parse(parts[9]); //altitude
                }
                catch (Exception)
                {
                }
                OnGpsData(m_gpsPoint);
            }

            else if (line.IndexOf("$GPRMC") == 0)
            {
                // Parse RMC - Recommended Minimum Navigation Information
                // $GPRMC,040302.663,A,3939.7,N,10506.6,W,0.27,358.86,200804,,*1A
                try
                {
                    string[] parts = line.Split(',');
                    if (parts.Length != 13 || parts[2] != "A")
                        return;

                    if (parts[9].Length == 6 && parts[1].Length == 10)
                    {
                        string date = parts[9]; // UTC Date DDMMYY
                        string time = parts[1]; // HHMMSS.XXX
                        int year = 2000 + int.Parse(date.Substring(4, 2));
                        int month = int.Parse(date.Substring(2, 2));
                        int day = int.Parse(date.Substring(0, 2));
                        int hour = int.Parse(time.Substring(0, 2));
                        int minute = int.Parse(time.Substring(2, 2));
                        int second = int.Parse(time.Substring(4, 2));
                        int milliseconds = int.Parse(time.Substring(7, 3));
                        m_gpsPoint.UtcTimestamp = new DateTime(year, month, day, hour, minute, second, milliseconds);
                    }

                    if (parts[3].Length == 9)
                    {
                        string lat = parts[3];  // HHMM.MMMM
                        float latHours = (float)Double.Parse(lat.Substring(0, 2));
                        float latMins = (float)Double.Parse(lat.Substring(2));
                        float latitude = latHours + latMins / 60.0f;
                        if (parts[4] == "S")       // N or S
                        {
                            latitude = -latitude;
                        }
                        m_gpsPoint.Latitude = latitude;
                    }

                    if (parts[5].Length == 10)
                    {
                        string lng = parts[5];  // HHHMM.M
                        float lngHours = (float)Double.Parse(lng.Substring(0, 3));
                        float lngMins = (float)Double.Parse(lng.Substring(3));
                        float longitude = lngHours + lngMins / 60.0f;
                        if (parts[6] == "W")
                        {
                            longitude = -longitude;
                        }
                        m_gpsPoint.Longitude = longitude;
                    }

                    m_gpsPoint.HorizontalSpeed = (float)Double.Parse(parts[7]) * 0.51444f; // knots to m/s
                    m_gpsPoint.Heading = (ushort)double.Parse(parts[8]);
                }
                catch (Exception)
                {
                    // One of our parses failed...ignore.
                }
                OnGpsData(m_gpsPoint);
            }
        }
    }
}
