<?php
	require_once('../config.inc.php');

	$today = gmdate('Y-m-d 00:00:00');
	if (isset($_GET['since'])) {
		$since = $_GET['since'];
	}
	else {
		$since = $today;
	}
	if (isset($_GET['blogsince'])) {
		$blogsince = $_GET['blogsince'];
	}
	else {
		$blogsince = $today;
	}
	
	try {
		$dbh = new PDO("mysql:host=$dbhost;dbname=$dbname", $dbuser, $dbpass);
		
		$telemetry = array();
		$lastimage = null;
		$blog = array();
		$gpspos = null;
		
		$stmt = $dbh->prepare('SELECT utctimestamp, latitude, longitude, galtitude, paltitude, heading, hspeed, vspeed, satellites, inttemperature, temperature1, temperature2, pressure, vin, gamma, gammacpm FROM live_telemetry WHERE utctimestamp>:since ORDER BY utctimestamp');
		$stmt->bindParam(':since', $since);
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$telemetry[] = array('utctimestamp' => $row['utctimestamp'],
									'latitude' => $row['latitude'],
									'longitude' => $row['longitude'],
									'galtitude' => $row['galtitude'],
									'paltitude' => $row['paltitude'],
									'heading' => $row['heading'],
									'hspeed' => $row['hspeed'],
									'vspeed' => $row['vspeed'],
									'satellites' => $row['satellites'],
									'inttemperature' => $row['inttemperature'],
									'temperature1' => $row['temperature1'],
									'temperature2' => $row['temperature2'],
									'pressure' => $row['pressure'],
									'vin' => $row['vin'],
									'gamma' => $row['gamma'],
									'gammacpm' => $row['gammacpm']);
		}
		
		$stmt = $dbh->prepare('SELECT utctimestamp, filename FROM live_images WHERE utctimestamp>:since ORDER BY utctimestamp DESC LIMIT 1');
		$stmt->bindParam(':since', $today);
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$lastimage = array('utctimestamp' => $row['utctimestamp'], 'filename' => $row['filename']);
		}
		
		$stmt = $dbh->prepare('SELECT utctimestamp, message FROM live_blog WHERE utctimestamp>:since ORDER BY utctimestamp');
		$stmt->bindParam(':since', $blogsince);
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$blog[] = array('utctimestamp' => $row['utctimestamp'],
									'message' => $row['message']);
		}
		
		$stmt = $dbh->prepare('SELECT utctimestamp, latitude, longitude FROM live_gpspos ORDER BY utctimestamp DESC LIMIT 1');
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$gpspos = array('utctimestamp' => $row['utctimestamp'],
									'latitude' => $row['latitude'],
									'longitude' => $row['longitude']);
		}
		
		$json = json_encode(array('telemetry' => $telemetry, 'liveimage' => $lastimage, 'blog' => $blog, 'gpspos' => $gpspos));
		
		echo $json;
	}
	catch (Exception $e) {
	}
?>
