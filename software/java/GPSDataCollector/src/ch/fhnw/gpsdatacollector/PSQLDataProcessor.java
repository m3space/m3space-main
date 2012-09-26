package ch.fhnw.gpsdatacollector;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import org.apache.log4j.Logger;

public class PSQLDataProcessor implements DataProcessor {

	
	private Connection conn = null;
	private PreparedStatement pstmt;
	private String dbHost;
	private int dbPort;
	private String dbName;
	private String dbUser;
	private String dbPass;
	
	// logging facility
	static Logger logger = Logger.getLogger(PSQLDataProcessor.class);
	
	
	public PSQLDataProcessor(String dbHost, int dbPort, String dbName, String dbUser, String dbPass) {	
		this.dbHost = dbHost;
		this.dbPort = dbPort;
		this.dbName = dbName;
		this.dbUser = dbUser;
		this.dbPass = dbPass;
	}
	
	
	public void initialize() throws ClassNotFoundException, SQLException {
		Class.forName("org.postgresql.Driver");
		createConnection();
		logger.debug("PostgreSQL data processor initialized.");
	}
	
	
	public void close() {
		if (conn != null) {
			try {
				conn.close();
			}
			catch (SQLException e) {
			}
		}
		logger.debug("PostgreSQL data processor closed.");
	}
	
	
	private boolean checkConnection() {
		
		if (conn != null) {
			try {
				boolean valid = conn.isValid(10);
				if (valid) {
					return true;
				}				
			}
			catch (SQLException e) {
			}
		}
		
		return createConnection();
	}
	
	
	
	private boolean createConnection() {
		try {
			conn = DriverManager.getConnection("jdbc:postgresql://" + dbHost + ":" + dbPort + "/" + dbName, dbUser, dbPass);
			pstmt = conn.prepareStatement("INSERT INTO gpsdata (datetime, phonenumber, valid, latitude, longitude, speed, heading, signalstrength, imei, satellites, altitude, batterystate, batteryvoltage, charging, mcc, mnc, lac, cellid) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);");
			logger.debug("Connection established.");
			return true;
		}
		catch (SQLException e) {
			logger.error("Database connection failed.");
			return false;
		}
	}
	
	
	@Override
	public void processGPSData(GPSDataSet data) throws DataProcessorException {
		
		if (checkConnection()) {
			try {
				java.sql.Timestamp ts = new java.sql.Timestamp(data.datetime.getTime());
				pstmt.setTimestamp(1, ts);
				pstmt.setString(2, data.phonenumber);
				pstmt.setBoolean(3, data.valid);
				pstmt.setDouble(4, data.latitude);
				pstmt.setDouble(5, data.longitude);
				pstmt.setDouble(6, data.speed);
				pstmt.setDouble(7, data.heading);
				pstmt.setShort(8, data.signalstrength);
				pstmt.setLong(9, data.imei);
				pstmt.setInt(10, data.satellites);
				pstmt.setDouble(11, data.altitude);
				pstmt.setShort(12, data.batterystate);
				pstmt.setDouble(13, data.batteryvoltage);
				pstmt.setBoolean(14, data.charging);
				pstmt.setInt(15, data.mcc);
				pstmt.setInt(16, data.mnc);
				pstmt.setInt(17, data.lac);
				pstmt.setInt(18, data.cellid);
				
				pstmt.executeUpdate();
				logger.info("New GPS data inserted.");
			
			}
			catch (SQLException e) {
				throw new DataProcessorException("SQL error during insert: " + e.getMessage());
			}
		}
	}


	@Override
	public void cleanUp() {
		try {
			conn.close();			
		}
		catch (Exception e) {
		}
	}

}
