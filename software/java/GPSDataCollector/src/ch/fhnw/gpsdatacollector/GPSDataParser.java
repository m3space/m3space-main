package ch.fhnw.gpsdatacollector;

import java.text.SimpleDateFormat;
import org.apache.log4j.Logger;

public class GPSDataParser {
	
	private static final int numFields = 28;
	private static final SimpleDateFormat dateformat = new SimpleDateFormat("yyMMddHHmmss");
	
	// logging facility
	static Logger logger = Logger.getLogger(GPSDataParser.class);
	
	
	public GPSDataParser() {
		
	}
	
	
	GPSDataSet parseLine(String line) {
		
		String[] fields = line.split(",");
		
		if (fields.length == numFields) {
			
			GPSDataSet data = new GPSDataSet();
			
			try {
				data.datetime = dateformat.parse(fields[0]);
				data.phonenumber = fields[1];
				data.valid = fields[4].equals("A");
				
				double l;
				l = Double.parseDouble(fields[5]);
				data.latitude = (int)(l / 100) + ((l - (int)(l / 100) * 100) / 60.0);
				if (fields[6].equals("S")) {
					data.latitude = -data.latitude;
				}
				
				l = Double.parseDouble(fields[7]);
				data.longitude = (int)(l / 100) + ((l - (int)(l / 100) * 100) / 60.0);
				if (fields[8].equals("W")) {
					data.latitude = -data.latitude;
				}
				
				data.speed = Double.parseDouble(fields[9]);
				data.heading = Double.parseDouble(fields[10]);
				
				data.signalstrength = fields[15].equals("F") ? (byte)1 : (byte)0;
				
				if (fields[17].startsWith(" imei:")) {
					data.imei = Long.parseLong(fields[17].substring(6));
				}
				else {
					throw new Exception("Invalid IMEI");
				}

                data.satellites = Integer.parseInt(fields[18]);
                data.altitude = Double.parseDouble(fields[19]);

                String[] bat = fields[20].split(":");
                if (bat.length > 1) {
                	data.batterystate = bat[0].equals("F") ? (byte)1 : (byte)0;
                	data.batteryvoltage = Double.parseDouble(bat[1].substring(0, bat[1].length() - 2));
                }

                data.charging = fields[21].equals("0") ? false : true;
                data.mcc = Integer.parseInt(fields[24]);
                data.mnc = Integer.parseInt(fields[25]);
                data.lac = Integer.parseInt(fields[26], 16);
                data.cellid = Integer.parseInt(fields[27], 16);
				
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
