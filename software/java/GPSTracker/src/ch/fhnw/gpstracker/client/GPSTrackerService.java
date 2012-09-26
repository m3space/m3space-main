package ch.fhnw.gpstracker.client;

import ch.fhnw.gpstracker.shared.GPSDataSet;
import ch.fhnw.gpstracker.shared.LoginResult;

import com.google.gwt.user.client.rpc.RemoteService;
import com.google.gwt.user.client.rpc.RemoteServiceRelativePath;

/**
 * The client side stub for the RPC service.
 */
@RemoteServiceRelativePath("track")
public interface GPSTrackerService extends RemoteService {
	
	Boolean isUserLoggedIn();
	LoginResult logUserIn(String username, String pwd);
	LoginResult logUserOut();
	Long[] getIMEIs();
	GPSDataSet[] getLastNPositions(int n, long imei);
	GPSDataSet[] getTodaysPositions(long imei);

}
