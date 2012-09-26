package ch.fhnw.gpstracker.client;

import java.util.ArrayList;
import java.util.Date;

import ch.fhnw.gpstracker.shared.GPSDataSet;
import ch.fhnw.gpstracker.shared.LoginResult;

import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ChangeHandler;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.logical.shared.SelectionEvent;
import com.google.gwt.event.logical.shared.SelectionHandler;
import com.google.gwt.i18n.client.DateTimeFormat;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.maps.client.MapWidget;
import com.google.gwt.maps.client.control.LargeMapControl;
import com.google.gwt.maps.client.control.MapTypeControl;
import com.google.gwt.maps.client.event.MarkerClickHandler;
import com.google.gwt.maps.client.geom.LatLng;
import com.google.gwt.maps.client.overlay.Marker;
import com.google.gwt.maps.client.overlay.Polyline;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.rpc.AsyncCallback;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DecoratedTabPanel;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasAlignment;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.PasswordTextBox;
import com.google.gwt.user.client.ui.VerticalPanel;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class GPSTracker implements EntryPoint {
	
	private static final int timerInterval = 30000;
	private static final int numPositions = 20;
	
	private static final NumberFormat degFormat = NumberFormat.getFormat("###.####");
	private static final NumberFormat distFormat = NumberFormat.getFormat("#.##");
	private static final DateTimeFormat dateFormat = DateTimeFormat.getFormat("yyyy/MM/dd HH:mm:ss");

	private final GPSTrackerServiceAsync trackerService = GWT
			.create(GPSTrackerService.class);

	private DecoratedTabPanel tabPanel = null;
	private HorizontalPanel mapPanel = null;
	private HTMLPanel aboutPanel = null;
	private VerticalPanel mapControlPanel = null;
	private HTMLPanel detailPanel = null;
	
	private Label lblTimestamp;
	private Label lblLatitude;
	private Label lblLongitude;
	private Label lblAltitude;
	private Label lblHeading;
	private Label lblAscentRate;
	private Label lblSatellites;

	private MapWidget gMap = null;

	private ArrayList<GPSDataSet> dataList;
	private Timer mapUpdater = null;

	private Button lastPosButton = null;
	private ListBox imeiList = null;
	
	private Long selectedIMEI;

	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		
		selectedIMEI = null;

		dataList = new ArrayList<GPSDataSet>();

		// tabs
		tabPanel = new DecoratedTabPanel();
		aboutPanel = new HTMLPanel("");
		mapPanel = new HorizontalPanel();
		detailPanel = new HTMLPanel("<table>" +
				"<tr><td><b>Time:</b></td><td id=\"timestamp\"></td></tr>" +
				"<tr><td><b>Lat.:</b></td><td id=\"latitude\"></td></tr>" +
				"<tr><td><b>Long.:</b></td><td id=\"longitude\"></td></tr>" +
				"<tr><td><b>Alt.:</b></td><td id=\"altitude\"></td></tr>" +
				"<tr><td><b>Heading:</b></td><td id=\"heading\"></td></tr>" +
				"<tr><td><b>Est. Ascent:</b></td><td id=\"ascentrate\"></td></tr>" +
				"<tr><td><b>Sat.:</b></td><td id=\"satellites\"></td></tr>" +
				"</table>");
		
		lblTimestamp = new Label("");
		lblLatitude = new Label("");
		lblLongitude = new Label("");
		lblAltitude = new Label("");
		lblHeading = new Label("");
		lblAscentRate = new Label("");
		lblSatellites = new Label("");
		detailPanel.add(lblTimestamp, "timestamp");
		detailPanel.add(lblLatitude, "latitude");
		detailPanel.add(lblLongitude, "longitude");
		detailPanel.add(lblAltitude, "altitude");
		detailPanel.add(lblHeading, "heading");
		detailPanel.add(lblAscentRate, "ascentrate");
		detailPanel.add(lblSatellites, "satellites");

		// map control container
		lastPosButton = new Button("Go to last position");
		imeiList = new ListBox(false);
		mapControlPanel = new VerticalPanel();
		mapControlPanel.add(imeiList);
		mapControlPanel.add(lastPosButton);
		mapControlPanel.add(detailPanel);
		

		// map
		gMap = new MapWidget();
		gMap.setCenter(LatLng.newInstance(47.4810, 8.2110), 13);
		gMap.addControl(new LargeMapControl());
		gMap.addControl(new MapTypeControl());
		gMap.setScrollWheelZoomEnabled(true);
		gMap.setSize("800px", "600px");

		// tabs		
		mapPanel.add(gMap);
		mapPanel.add(mapControlPanel);
		
		
		tabPanel.setWidth("900px");
		tabPanel.setAnimationEnabled(true);
		tabPanel.add(mapPanel, "Map");
		tabPanel.add(aboutPanel, "About");
		tabPanel.selectTab(0);
		tabPanel.setVisible(true);
		
		RootPanel.get("gContainer").add(tabPanel);

		/*
		 * 
		 * Event handlers
		 */
		lastPosButton.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent ev) {
				if (dataList.size() > 0) {
					gMap.setCenter(LatLng.newInstance(dataList.get(0).latitude,
							dataList.get(0).longitude));
					updateDetails(dataList.get(0));
				}
			}
		});

		tabPanel.addSelectionHandler(new SelectionHandler<Integer>() {
			public void onSelection(SelectionEvent<Integer> event) {
				if (event.getSelectedItem() == 1)
					gMap.checkResizeAndCenter();
			}

		});
		
		imeiList.addChangeHandler(new ChangeHandler() {
			public void onChange(ChangeEvent event)	{
				int idx = imeiList.getSelectedIndex();
				if (idx >= 0) {
					try {
						selectedIMEI = Long.parseLong(imeiList.getItemText(idx));
						reloadMap(selectedIMEI);
						
					} catch (NumberFormatException e) {
						ErrorBox("invalid IMEI!");
					}
				}
				else {
					unloadMap();
				}
			}
		});

		/*trackerService.isUserLoggedIn(new AsyncCallback<Boolean>() {
			public void onFailure(Throwable caught) {
				ErrorBox("User verification fail!");
			}

			public void onSuccess(Boolean result) {
				if (result) {
					reloadMap(selectedIMEI);
					return;
				} else {
					login();

				}

			}
		});*/
		
		updateIMEIs();

	}

	public void reloadMap(long imei) {

		stopLiveTracking();
		updatePostions(imei);
		startLiveTracking(imei);
	}
	
	
	public void unloadMap() {
		stopLiveTracking();
		dataList.clear();
		gMap.clearOverlays();
	}

	private void startLiveTracking(final long imei) {
		if (mapUpdater != null) {
			ErrorBox("Timer already running");
		}

		mapUpdater = new Timer() {
			public void run() {
				updatePostions(imei);
			}
		};
		mapUpdater.scheduleRepeating(timerInterval);

	}

	private void stopLiveTracking() {

		if (mapUpdater != null) {
			mapUpdater.cancel();
		}
		mapUpdater = null;
	}

	public void login() {
		TextDialog d = new TextDialog("Login...");
		final TextBox nameField = new TextBox();
		Label nameLabel = new Label("Username:");
		final PasswordTextBox pwdField = new PasswordTextBox();
		Label pwdLabel = new Label("Password:");
		d.VPanel.add(nameLabel);
		d.VPanel.setCellHorizontalAlignment(nameLabel,
				HasAlignment.ALIGN_CENTER);
		d.VPanel.add(nameField);
		d.VPanel.setCellHorizontalAlignment(nameField,
				HasAlignment.ALIGN_CENTER);
		d.VPanel.add(pwdLabel);
		d.VPanel.setCellHorizontalAlignment(pwdLabel, HasAlignment.ALIGN_CENTER);
		d.VPanel.add(pwdField);
		d.VPanel.setCellHorizontalAlignment(pwdField, HasAlignment.ALIGN_CENTER);
		d.show();

		d.OKButton.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent e) {
				String username = nameField.getText();
				String pwd = pwdField.getText();

				trackerService.logUserIn(username, pwd,
						new AsyncCallback<LoginResult>() {
							public void onFailure(Throwable caught) {
								ErrorBox("Login service fail!");
							}

							public void onSuccess(LoginResult result) {
								if (!result.isSuccessful()) {
									login();
									ErrorBox("Username/Password wrong!");
								} else {
									// StatusBox(result.toString() + "//" +
									// "Logged in: " + nameField.getText() +
									// ":" + pwdField.getText());
									tabPanel.setVisible(true);

									updateIMEIs();
								}
							}
						});
			}
		});
	}

	public void showDialog(String title, String msg) {
		TextDialog d = new TextDialog();
		d.setText(title);
		d.VPanel.add(new Label(msg));
		d.show();
	}

	public void ErrorBox(String msg) {
		showDialog("Error", msg);
	}

	public void StatusBox(String msg) {
		showDialog("Status", msg);
	}
	

	private static class TextDialog extends DialogBox {
		public VerticalPanel VPanel;
		public VerticalPanel VBPanel;
		public Button OKButton;

		public TextDialog() {
			Init();
		}

		public TextDialog(String t) {
			Init();
			setText(t);
		}

		private void Init() {
			VPanel = new VerticalPanel();
			VBPanel = new VerticalPanel();
			VerticalPanel myPanel = new VerticalPanel();

			myPanel.add(VPanel);
			myPanel.setHorizontalAlignment(VerticalPanel.ALIGN_RIGHT);
			myPanel.add(VBPanel);

			OKButton = new Button("OK");
			OKButton.addClickHandler(new ClickHandler() {
				public void onClick(ClickEvent e) {
					TextDialog.this.hide();
				}
			});

			myPanel.add(OKButton);
			setWidget(myPanel);
			center();
		}
	}
	
	private class GPSDetailHandler implements MarkerClickHandler {
		
		GPSDataSet data;
		GPSTracker ui;
		
		public GPSDetailHandler(GPSDataSet data, GPSTracker ui) {
			this.data = data;
			this.ui = ui;
		}
		
		@Override
		public void onClick(MarkerClickEvent event) {
			ui.updateDetails(data);
		}
		
	}

	private void updatePostions(long imei)	{
		
		//trackerService.getTodaysPositions(imei, new AsyncCallback<GPSDataSet[]>() {
		trackerService.getLastNPositions(numPositions, imei, new AsyncCallback<GPSDataSet[]>() {
					public void onFailure(Throwable caught) {
						ErrorBox("GPS data retrieval fail!");	
					}
					public void onSuccess(GPSDataSet[] result) {
						if(result == null)	{
							ErrorBox("Not logged in!");
							return;
						}
						
						if (result.length == 0) {
							ErrorBox("No GPS data!");
							return;
						}

						dataList.clear();
						gMap.clearOverlays();
						LatLng latlon[] = new LatLng[result.length];
						for (int i = 0; i < result.length; ++i) {
								
							latlon[i] = LatLng.newInstance((result[i]).latitude,result[i].longitude);
							dataList.add(result[i]);
							//ErrorBox("ls: " + PositionList.size() + " p:" + result[i].getLatitude());
							Marker marker = new Marker(latlon[i]);
							
							marker.addMarkerClickHandler(new GPSDetailHandler(result[i], GPSTracker.this));
							
							gMap.addOverlay(marker);
								
						}
						Polyline p  = new Polyline(latlon);
						gMap.addOverlay(p);	
						gMap.setCenter(latlon[0]);
						
						updateDetails(result[0]);
						
						GWT.log("updated:"+result.length,null);
						//ErrorBox("got " + PositionList.size() + " entrys");
					}

			});
	}
	
	
	public void updateDetails(GPSDataSet data) {
		lblTimestamp.setText(dateFormat.format(data.datetime));
		lblLatitude.setText(degFormat.format(data.latitude));
		lblLongitude.setText(degFormat.format(data.longitude));
		lblAltitude.setText(distFormat.format(data.altitude) + " m");
		lblHeading.setText(degFormat.format(data.heading));
		lblAscentRate.setText(distFormat.format(data.ascentrate) + " m/s");
		lblSatellites.setText(Integer.toString(data.satellites));
	}

	private void updateIMEIs() {
		
		trackerService.getIMEIs(new AsyncCallback<Long[]>() {

			@Override
			public void onFailure(Throwable caught) {
				ErrorBox("Device list retrieval fail!");
			}

			@Override
			public void onSuccess(Long[] result) {
				if(result == null)	{
					ErrorBox("Not logged in");
					return;
				}
				
				for (int i = 0; i < result.length; i++) {
					imeiList.addItem(result[i].toString());
				}
				
				imeiList.setSelectedIndex(-1);
			}
			
		});
	}

}
