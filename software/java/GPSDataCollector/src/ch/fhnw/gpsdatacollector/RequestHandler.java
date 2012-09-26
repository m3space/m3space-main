package ch.fhnw.gpsdatacollector;

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
		WAIT_SYNC
	};
	

	private Socket socket;
	private InputStream is;
	private boolean doRun;
	private GPSDataParser parser;
	private DataProcessor dproc;
	private byte[] rcvBuf;
	private byte[] lineBuf;
	private int linePos;
	private ReceiverState rcvState;
	
	static final int rcvBufSize = 512;
	static final int lineBufSize = 1024;
	
	// logging facility
	static Logger logger = Logger.getLogger(RequestHandler.class);
	
	/**
	 * Creates a handler and the helper objects.
	 * @param s Socket for the request
	 * @param dp Data processor object
	 */
	public RequestHandler(Socket s, DataProcessor dp) throws IOException {
		
		socket = s;
		dproc = dp;
		parser = new GPSDataParser();

		//get streams
		is = s.getInputStream();
		
		rcvBuf = new byte[rcvBufSize];
		lineBuf = new byte[lineBufSize];
		linePos = 0;
		rcvState = ReceiverState.WAIT_BEGIN;
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
		

		dproc.cleanUp();

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
				receiveByte(rcvBuf[i]);
			}
		}
		else if ((n < 0) || socket.isClosed()) {
			logger.debug("Cannot read data.");
			disconnect();
		}
	}
	
	
	/**
	 * Process a received byte.
	 * @param b the received byte
	 */
	private void receiveByte(byte b) {
		switch (rcvState) {
		
			case WAIT_BEGIN:
				// line starts with a digit (0..9)
				if ((b >= 48) && (b <= 57)) {
					rcvState = ReceiverState.DATA_RECEIVING;
					linePos = 0;
					readDataByte(b);
				}
				break;
				
			case DATA_RECEIVING:
				// LF (endline is LFCR instead of CRLF)
				if (b == 0x0A) {
					rcvState = ReceiverState.WAIT_SYNC;
					logger.debug("Line complete.");
					processLine();
				}
				else {
					readDataByte(b);
				}
				break;
				
			case WAIT_SYNC:
				// wait for CR 
				if (b == 0x0D) {
					rcvState = ReceiverState.WAIT_BEGIN;
					logger.debug("Ready to receive.");
				}
				break;
				
			default:
				// something wrong
				rcvState = ReceiverState.WAIT_SYNC;
				logger.debug("Undefined receiver state.");
				break;
		}
	}

	
	/**
	 * Puts a data byte into the buffer.
	 * @param b the data byte
	 */
	private void readDataByte(byte b) {
		if (linePos < lineBufSize) {
			lineBuf[linePos++] = b;
		}
		else {
			rcvState = ReceiverState.WAIT_SYNC;
		}
	}
	
	
	/**
	 * Processes a line of text containing the GPS data.
	 * Data is taken from the current line buffer.
	 */
	private void processLine() {
		// this method works because received data is in 8bit ASCII format
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < linePos; i++) {
			sb.append((char)lineBuf[i]);
		}
		String line = sb.toString();
		logger.debug("Line received: " + line);
		GPSDataSet gpsdata = parser.parseLine(line);
		if (gpsdata != null) {
			try {
				dproc.processGPSData(gpsdata);
			}
			catch (DataProcessorException e) {
				logger.error("Error while processing line: " + e.getMessage());
			}
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
