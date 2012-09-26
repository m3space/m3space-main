package ch.fhnw.gpsdatacollector.test;

import java.text.SimpleDateFormat;
import org.apache.log4j.Logger;

import ch.fhnw.gpsdatacollector.GPSDataSet;

public class SMSGPSDataParser {
	
	private static final int numFields = 17;
	private static final SimpleDateFormat dateformat = new SimpleDateFormat("dd/MM/yyHH:mm");
	
	// logging facility
	static Logger logger = Logger.getLogger(SMSGPSDataParser.class);
	
	
	public SMSGPSDataParser() {
		
	}
	
	
	GPSDataSet parseLine(String line) {
		
		String[] fields = line.split(" ");
		
		if (fields.length == numFields) {
			
			if (!fields[0].startsWith("lat")) {
				logger.error("Not a valid GPS SMS.");
				return null;
			}
			
			GPSDataSet data = new GPSDataSet();
			
			try {
				// not sent in SMS text
				data.phonenumber = "";
				data.valid = true;
				data.heading = 0.0;
				
				// decimal degrees with N or S
				data.latitude = Double.parseDouble(fields[1].substring(0, fields[1].length()-2));
				if (fields[1].endsWith("S")) {
					data.latitude = -data.latitude;
				}
				
				// decimal degrees with E or W
				data.longitude = Double.parseDouble(fields[3].substring(0, fields[3].length()-2));
				if (fields[3].endsWith("W")) {
					data.latitude = -data.latitude;
				}
				
				// km/h!
				data.speed = Double.parseDouble(fields[5]);
				
				// no seconds
				data.datetime = dateformat.parse(fields[6] + fields[7]);
				
				String[] bat = fields[8].split(":|,");
                if (bat.length >= 3) {
                	// only good (F) or bad (L)
                	data.batterystate = bat[0].equals("F") ? (byte)1 : (byte)0;
                	// volts
                	data.batteryvoltage = Double.parseDouble(bat[1].substring(0, bat[1].length()-2));
                	// 1 or 0
                	data.charging = fields[2].equals("0") ? false : true;
                }
                else {
					throw new Exception("Invalid battery state");
				}
				
                // only good (F) or bad (L)
				data.signalstrength = fields[9].endsWith("F") ? (byte)1 : (byte)0;
				
				// GSM IMEI
				if (fields[10].startsWith("imei:")) {
					data.imei = Long.parseLong(fields[10].substring(5));
				}
				else {
					throw new Exception("Invalid IMEI");
				}

				// number of satellites in range
                data.satellites = Integer.parseInt(fields[11]);
                
                // meters over sea level
                data.altitude = Double.parseDouble(fields[12]);
                
                // country code
                data.mcc = Integer.parseInt(fields[13]);
                
                // GSM network code
                data.mnc = Integer.parseInt(fields[14]);
                
                // location area code
                data.lac = Integer.parseInt(fields[15], 16);
                
                // GSM cell ID
                data.cellid = Integer.parseInt(fields[16], 16);
				
				return data;
			}
			catch (Exception e) {
				logger.error("Error while parsing data: " + e.getMessage());
				return null;
			}
			
		}
		
		logger.error(fields.length + " data fields read instead of " + numFields + "!");
		return null;
	}
}
