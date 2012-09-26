package ch.fhnw.gpsdatacollector;

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Properties;

import org.apache.log4j.Logger;


/**
 * This class is the main class of the GPSDataCollector server.
 * It accepts connection requests and handles them.
 * @author Matthias Krebs
 * Datum: 04.07.2011
 *
 */
public class GPSDataCollector {

	// logging facility
	static Logger logger = Logger.getLogger(GPSDataCollector.class);
	
	// server configuration
	static final String propertyFile = "server.properties";

	
	private static ServerSocket socket;

	/**
	 * Program entry point
	 * @param args	commandline parameters
	 */
	public static void main(String[] args) {
		
		int tcpPort = -1;
		int socketTimeout = 0;
		String dbHost;
		int dbPort = -1;
		String dbDatabase;
		String dbUser;
		String dbPass;
		
		// load properties
		try {
			Properties properties = new Properties();
			BufferedInputStream stream = new BufferedInputStream(new FileInputStream(propertyFile));
			properties.load(stream);
			stream.close();
			String tcpPortStr = properties.getProperty("TCPPort");
			tcpPort = Integer.parseInt(tcpPortStr);
			String timeoutStr = properties.getProperty("Timeout");
			socketTimeout = Integer.parseInt(timeoutStr);
			dbHost = properties.getProperty("DBHost");
			dbPort = Integer.parseInt(properties.getProperty("DBPort"));
			dbDatabase = properties.getProperty("DBDatabase");
			dbUser = properties.getProperty("DBUser");
			dbPass = properties.getProperty("DBPass");
			
		}
		catch (IOException e) {
			logger.fatal("Property file '" + propertyFile + "' not found.");
			return;
		}
		catch (NumberFormatException e) {
			logger.fatal("Invalid format in property file '" + propertyFile + "'.");
			return;
		}
		StringBuilder sb = new StringBuilder();
		sb.append("Server configuration:\n");
		sb.append("TCP Port: " + tcpPort + "\n");
		sb.append("Timeout: " + socketTimeout + "\n");
		sb.append("Database host: " + dbHost + ":" + dbPort + "\n");
		sb.append("Database: " + dbDatabase);
		logger.debug(sb.toString());
		

		// start TCP server
		try {
			socket = new ServerSocket(tcpPort);
			logger.info("Server listening on port " + tcpPort + ".");
			Socket s;
			while (true) {
				// new client connection
				s = socket.accept();
				s.setSoLinger(false, 0);
				s.setSoTimeout(socketTimeout);
				DataProcessor dp = new PSQLDataProcessor(dbHost, dbPort, dbDatabase, dbUser, dbPass);
				Thread t = new Thread(new RequestHandler(s, dp));
				t.start();
			}
		}
		catch (IOException e) {
			logger.error(e.getMessage());
		}
		
		logger.info("Server shutdown.");
	}
}
