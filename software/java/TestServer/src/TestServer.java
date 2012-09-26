

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Properties;

import org.apache.log4j.Logger;

public class TestServer {

	// logging facility
	static Logger logger = Logger.getLogger(TestServer.class);
	
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
				Thread t = new Thread(new RequestHandler(s));
				t.start();
			}
		}
		catch (IOException e) {
			logger.error(e.getMessage());
		}
		
		logger.info("Server shutdown.");
	}
}
