package ch.fhnw.gpstracker.shared;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class Encryption {
	
	/**
	 * Generate SHA-1 hash
	 */
	public static String generateSHA1(String text) {
		MessageDigest md;
		try {
			md = MessageDigest.getInstance("SHA-1");
		}
		catch (NoSuchAlgorithmException e) {
			return null;
		}
		byte[] sha1hash = new byte[20];
		md.update(text.getBytes());
		sha1hash = md.digest();
		
		int h;
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < 20; i++) {
			h = sha1hash[i] & 0xff;
			sb.append((h < 0x10) ? "0${Integer.toHexString(h)}" : Integer.toHexString(h));
		}
		
		return sb.toString();
	}
}
