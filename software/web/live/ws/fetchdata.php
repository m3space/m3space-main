<?php
	require_once('../config.inc.php');

	$today = gmdate('Y-m-d 00:00:00');
	if (isset($_GET['since'])) {
		$since = $_GET['since'];
	}
	else {
		$since = $today;
	}
	
	try {
		$dbh = new PDO("mysql:host=$dbhost;dbname=$dbname", $dbuser, $dbpass);
		
		$telemetry = array();
		$lastimage = null;
		
		$stmt = $dbh->prepare('SELECT utctimestamp, latitude, longitude, galtitude, paltitude, heading, speed, satellites, inttemperature, exttemperature, pressure, vin FROM live_telemetry WHERE utctimestamp>:since ORDER BY utctimestamp');
		$stmt->bindParam(':since', $since);
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$telemetry[] = array('utctimestamp' => $row['utctimestamp'],
									'latitude' => $row['latitude'],
									'longitude' => $row['longitude'],
									'galtitude' => $row['galtitude'],
									'paltitude' => $row['paltitude'],
									'heading' => $row['heading'],
									'speed' => $row['speed'],
									'satellites' => $row['satellites'],
									'inttemperature' => $row['inttemperature'],
									'exttemperature' => $row['exttemperature'],
									'pressure' => $row['pressure'],
									'vin' => $row['vin']);
		}
		
		$stmt = $dbh->prepare('SELECT utctimestamp, filename FROM live_images WHERE utctimestamp>:since ORDER BY utctimestamp DESC LIMIT 1');
		$stmt->bindParam(':since', $today);
		$stmt->execute();
		while ($row = $stmt->fetch()) {
			$lastimage = array('utctimestamp' => $row['utctimestamp'], 'filename' => $row['filename']);
		}

		$json = json_encode(array('telemetry' => $telemetry, 'liveimage' => $lastimage));
		
		echo $json;
	}
	catch (Exception $e) {
	}
?>
