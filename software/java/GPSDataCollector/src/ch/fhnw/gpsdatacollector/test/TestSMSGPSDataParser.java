package ch.fhnw.gpsdatacollector.test;

import ch.fhnw.gpsdatacollector.GPSDataSet;

public class TestSMSGPSDataParser {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
		String testLine = "lat: 47.545907N long: 7.562793E speed: 0.00 30/06/11 19:44 F:4.20V,0, Signal:F imei:012207006348079 07 299.0 228 01 05EB 04DD";
		
		String[] fields = testLine.split(" |,");
		
		for (String str : fields) {
			System.out.println(str);
		}
		
		SMSGPSDataParser parser = new SMSGPSDataParser();
		GPSDataSet gpsdata = parser.parseLine(testLine);
	}

}
