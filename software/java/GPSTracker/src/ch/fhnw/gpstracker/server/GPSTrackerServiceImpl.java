package ch.fhnw.gpstracker.server;

import ch.fhnw.gpstracker.client.GPSTrackerService;
import ch.fhnw.gpstracker.shared.LoginResult;
import ch.fhnw.gpstracker.shared.Encryption;
import ch.fhnw.gpstracker.shared.GPSDataSet;

import com.google.gwt.user.server.rpc.RemoteServiceServlet;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.LinkedList;

/**
 * The server side implementation of the RPC service.
 */
@SuppressWarnings("serial")
public class GPSTrackerServiceImpl extends RemoteServiceServlet implements
		GPSTrackerService {
	
	private static final DateFormat dayFormat = new SimpleDateFormat("yyyy-MM-dd");
	
	private static final String dbHost = "datalogger.cs.technik.fhnw.ch";
	private static final String dbName = "gpsdata";
	private static final String dbUser = "gpsdata";
	private static final String dbPass = "gps$data11";
	
	private Integer userLoggedIn;
	

	public GPSTrackerServiceImpl() {
		userLoggedIn = null;
		
		try {
			Class.forName("org.postgresql.Driver");
		} catch (ClassNotFoundException e) {
			System.out.println("PostgreSQL driver not found!");
		}
	}
	
	
	private Connection dbOpen() {

		try {			
			return DriverManager.getConnection("jdbc:postgresql://" + dbHost + "/" + dbName, dbUser, dbPass);
		} catch (Exception e) {
			return null;
		}
	}


	public Boolean isUserLoggedIn() {
		return (userLoggedIn != null);
	}
	

	public LoginResult logUserIn(String Username, String password) {
		Connection con = dbOpen();

		PreparedStatement mreqUserLogin;

		Integer userid = null;
		
		try {
			
			mreqUserLogin = con
					.prepareStatement("SELECT id, password FROM users WHERE username=?");

			mreqUserLogin.setString(1, Username);
			ResultSet res = mreqUserLogin.executeQuery();

			if (res.next()) {
				int id = res.getInt("id");
				String dbpass = res.getString("password");
				
				if (dbpass.equals(Encryption.generateSHA1(password))) {
							
					userid = id;
				}
			}
			mreqUserLogin.close();
			con.close();

		} catch (SQLException e) {
			e.printStackTrace();
			return new LoginResult(null);
		}

		return new LoginResult(userid);
	}

	
	public LoginResult logUserOut() {
		Integer userid = userLoggedIn;
		userLoggedIn = null;
		return new LoginResult(userid);
	}
	
	
	public Long[] getIMEIs() {
		/*if (userLoggedIn == null) {
			return null;
		}*/
		
		LinkedList<Long> g = new LinkedList<Long>();
		
		try {		
			Connection con = dbOpen();
			PreparedStatement mresgps = con
			.prepareStatement("SELECT imei FROM devices ORDER BY imei");
			/*PreparedStatement mresgps = con
					.prepareStatement("SELECT imei FROM devices WHERE user_id=? ORDER BY imei");
			
			mresgps.setInt(1, userLoggedIn);*/
			ResultSet res = mresgps.executeQuery();

			while (res.next()) {
				g.add(res.getLong("imei"));
			}
			mresgps.close();
			con.close();
		} catch (SQLException e) {
			System.out.println("Error on getIMEIs");
			return null;
		}

		return g.toArray(new Long[g.size()]);
	}
	
	
	public GPSDataSet[] getLastNPositions(int n, long imei) {
		/*if (userLoggedIn == null) {
			return null;
		}*/
		
		LinkedList<GPSDataSet> g = new LinkedList<GPSDataSet>();
		
		try {		
			Connection con = dbOpen();
			PreparedStatement mresgps = con
					.prepareStatement("SELECT datetime, latitude, longitude, speed, heading, altitude, satellites FROM gpsdata WHERE imei=? AND valid=TRUE AND signalstrength>0 ORDER BY datetime DESC LIMIT ?");
			
			mresgps.setLong(1, imei);
			mresgps.setInt(2, n);
			ResultSet res = mresgps.executeQuery();

			GPSDataSet prev = null;
			long dt;
			
			// last position comes first
			while (res.next()) {
				GPSDataSet d = new GPSDataSet();
				d.datetime = res.getTimestamp("datetime");
				d.latitude = res.getDouble("latitude");
				d.longitude = res.getDouble("longitude");
				d.speed = res.getDouble("speed");
				d.heading = res.getDouble("heading");
				d.altitude = res.getDouble("altitude");
				d.satellites = res.getInt("satellites");
				
				if (prev != null) {
					// m/s
					dt = prev.datetime.getTime() - d.datetime.getTime();
					if (dt != 0) {
						prev.ascentrate = (prev.altitude - d.altitude) * 1000 / dt;
					}
				}
				
				g.add(d);
				
				prev = d;
			}
			mresgps.close();
			con.close();
		} catch (SQLException e) {
			System.out.println("Error on getlastnposition");
			return null;
		}

		return g.toArray(new GPSDataSet[g.size()]);

	}

	public GPSDataSet[] getTodaysPositions(long imei) {
		
		/*if (userLoggedIn == null) {
			return null;
		}*/
		
		LinkedList<GPSDataSet> g = new LinkedList<GPSDataSet>();
		
		try {
			Connection con = dbOpen();			
			PreparedStatement mresgps = con.prepareStatement("SELECT datetime, latitude, longitude, speed, heading, altitude, satellites FROM data WHERE imei=? AND valid=TRUE AND signalstrength>0 AND datetime>=? ORDER BY datetime DESC");
			mresgps.setLong(1, imei);
			mresgps.setString(2, dayFormat.format(new Date()));
			System.out.println(mresgps.toString());
			ResultSet res = mresgps.executeQuery();

			while (res.next()) {
				GPSDataSet d = new GPSDataSet();
				d.datetime = res.getTimestamp("datetime");
				d.latitude = res.getDouble("latitude");
				d.longitude = res.getDouble("longitude");
				d.speed = res.getDouble("speed");
				d.heading = res.getDouble("heading");
				d.altitude = res.getDouble("altitude");
				d.satellites = res.getInt("satellites");
				g.add(d);
			}
			mresgps.close();
			con.close();
		} catch (SQLException e) {
			System.out.println("Error on getlastnposition");
			return null;
		}

		return g.toArray(new GPSDataSet[g.size()]);
	}

}
