package ch.fhnw.gpstracker.shared;

import java.util.Date;

import com.google.gwt.user.client.rpc.IsSerializable;

public class GPSDataSet implements IsSerializable {
	
	public Date datetime;
	public double latitude;
	public double longitude;
	public double speed;
	public double heading;
	public double altitude;
	public double ascentrate = 0.0;
	public int satellites;
}
