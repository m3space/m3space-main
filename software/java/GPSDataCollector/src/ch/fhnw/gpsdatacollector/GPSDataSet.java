package ch.fhnw.gpsdatacollector;

import java.util.Date;

public class GPSDataSet {
	
	public Date datetime;
	public String phonenumber;
	public boolean valid;
	public double latitude;
	public double longitude;
	public double speed;
	public double heading;
	public byte signalstrength;
	public long imei;
	public int satellites;
	public double altitude;
	public byte batterystate;
	public double batteryvoltage;
	public boolean charging;
	public int mcc;
	public int mnc;
	public int lac;
	public int cellid;

	public GPSDataSet() {
		
	}
}
