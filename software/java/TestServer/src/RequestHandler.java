

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.util.Date;
import org.apache.log4j.Logger;


/**
 * The RequestHandler reads raw data and parses it.
 * @author Matthias Krebs
 * Datum: 27.07.2011
 *
 */
public class RequestHandler implements Runnable {
	
	enum ReceiverState {
		WAIT_BEGIN,
		DATA_RECEIVING,
		WAIT_ENDLINE,
		WAIT_SYNC
	};
	

	private Socket socket;
	private InputStream is;
	private boolean doRun;
	private byte[] rcvBuf;
	private FileWriter writer;

	
	static final int rcvBufSize = 512;

	
	// logging facility
	static Logger logger = Logger.getLogger(RequestHandler.class);
	

	public RequestHandler(Socket s) throws IOException {
		
		socket = s;
		writer = new FileWriter(new File("testoutput.txt"));

		//get streams
		is = s.getInputStream();
		
		rcvBuf = new byte[rcvBufSize];
	}
	
	@Override
	public void run() {
		Date now;
		try {
			now = new Date();
			InetAddress clientAddress = socket.getInetAddress();
			logger.info(now + ": New connection from client " + clientAddress + ".");
			doRun = true;
			while (doRun) {
				readData();
			}
			now = new Date();
			logger.info(now + ": Connection thread loop of client " + clientAddress + " ended.");
		}
		catch (IOException e) {
			now = new Date();
			try {
				//try to close the socket
				socket.close();
				logger.debug(now + ": Client socket closed after IOException.");
			}
			catch (IOException e1) {
				logger.debug(now + ": Failed to close socket after IOException.");
			}
		}

	}

	
	
	/**
	 * Reads data from the socket.
	 * @throws IOException
	 */
	private void readData() throws IOException {
		
		int n = is.read(rcvBuf, 0, rcvBufSize);
		
		if (n > 0) {
			logger.debug("Read " + n + " bytes.");
			for (int i = 0; i < n; i++) {
				writer.write(rcvBuf[i]);
			}
			writer.flush();
		}
		else if ((n < 0) || socket.isClosed()) {
			logger.debug("Cannot read data.");
			disconnect();
		}
	}
	
	
	/**
	 * Closes the socket.
	 */
	private void disconnect() {
		Date now = new Date();
		try {			
			socket.close();
			doRun = false;
			logger.info(now + ": Client socket closed by disconnect().");
		}
		catch (IOException e) {
			logger.warn(now + ": Failed to close client socket by disconnect().");
		}
	}
}
