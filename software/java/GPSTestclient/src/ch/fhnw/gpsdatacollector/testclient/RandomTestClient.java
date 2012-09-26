package ch.fhnw.gpsdatacollector.testclient;

import java.io.IOException;
import java.io.OutputStream;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Random;

public class RandomTestClient {
	
	static final String host = "datalogger.imvs.technik.fhnw.ch";
	static final int port = 10009;
	static final int bufsize = 168;
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
		
		byte[] buf = new byte[bufsize];
		Random rnd = new Random();
		
		while (true) {
			
			rnd.nextBytes(buf);
			
			try {
				out.write(buf);
				System.out.println("Sent " + bufsize + " bytes.");
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
