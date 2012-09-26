package ch.fhnw.gpsdatacollector.testclient;

import java.io.IOException;
import java.io.OutputStream;
import java.net.Socket;
import java.net.UnknownHostException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Random;

public class GPSTestClient {
	
	
	static final DateFormat dateFormat = new SimpleDateFormat("yyMMddHHmmss");
	static final String host = "datalogger.imvs.technik.fhnw.ch";
	static final int port = 10009;
	static final long interval = 10000;

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
		Socket socket = null;
		OutputStream out = null;
		
		try {
			socket = new Socket(host, port);
			out = socket.getOutputStream();
		}
		catch (UnknownHostException e) {
			System.out.println("Unknown host.");
			System.exit(-1);
		}
		catch (IOException e) {
			System.out.println("Error opening socket.");
			System.exit(-1);
		}
		
		Random rnd = new Random();
		
		while (true) {
			
			// create fake gps info
			Date now = new Date();			
			String str = dateFormat.format(now) + ",+41796666666,GPRMC,104531.000,X,4732.7567,N,00733.7707,E,0.00,187.91,030711,,,A*6F,F,, imei:123456789012345,09,306.7,F:3.87V,0,140,24970,228,01,0EFE,0697\n\r";			
			byte[] buf = str.getBytes();
			int len = buf.length;
			
			int x = rnd.nextInt(10);
			if (x > 5) {
				len /= 2;
			}
			
			try {
				out.write(buf, 0, len);
				System.out.println("Sent " + len + " bytes.");
			}
			catch (IOException e) {
				System.out.println("Send failed.");
				System.exit(-1);
			}
			
			try {
				Thread.sleep(interval);
			}
			catch (InterruptedException e) {
				System.out.println("Sleep interrupted.");
			}
			
		}

	}

}
