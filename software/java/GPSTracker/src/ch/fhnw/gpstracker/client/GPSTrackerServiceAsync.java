package ch.fhnw.gpstracker.client;

import ch.fhnw.gpstracker.shared.GPSDataSet;
import ch.fhnw.gpstracker.shared.LoginResult;

import com.google.gwt.user.client.rpc.AsyncCallback;

/**
 * The async counterpart of <code>GPSTrackerService</code>.
 */
public interface GPSTrackerServiceAsync {
	void isUserLoggedIn(AsyncCallback<Boolean> callback);
	void logUserIn(String username, String pwd,AsyncCallback<LoginResult> callback);
	void logUserOut(AsyncCallback<LoginResult> callback);
	void getIMEIs(AsyncCallback<Long[]> callback);	
	void getLastNPositions(int n, long imei, AsyncCallback<GPSDataSet[]> asyncCallback );
	void getTodaysPositions(long imei, AsyncCallback<GPSDataSet[]> asyncCallback);
}
