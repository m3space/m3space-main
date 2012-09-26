<?php
	require_once('../config.inc.php');
	
	if ($_SERVER['REQUEST_METHOD'] != 'POST') {
		http_response_code(400);
		exit();
	}
	
	$key = $_POST['key'];
	if ($key != $authkey) {
		http_response_code(403);
		exit();
	}
	
	try {
		$dbh = new PDO("mysql:host=$dbhost;dbname=$dbname", $dbuser, $dbpass);
		$stmt = $dbh->prepare('INSERT INTO live_telemetry (utctimestamp, latitude, longitude, galtitude, paltitude, heading, speed, satellites, inttemperature, exttemperature, pressure, vin) value (:utctimestamp, :latitude, :longitude, :galtitude, :paltitude, :heading, :speed, :satellites, :inttemperature, :exttemperature, :pressure, :vin)');
		$stmt->bindParam(':utctimestamp', $_POST['utctimestamp']);
		$stmt->bindParam(':latitude', $_POST['latitude']);
		$stmt->bindParam(':longitude', $_POST['longitude']);
		$stmt->bindParam(':galtitude', $_POST['galtitude']);
		$stmt->bindParam(':paltitude', $_POST['paltitude']);
		$stmt->bindParam(':heading', $_POST['heading']);
		$stmt->bindParam(':speed', $_POST['speed']);
		$stmt->bindParam(':satellites', $_POST['satellites']);
		$stmt->bindParam(':inttemperature', $_POST['inttemperature']);
		$stmt->bindParam(':exttemperature', $_POST['exttemperature']);
		$stmt->bindParam(':pressure', $_POST['pressure']);
		$stmt->bindParam(':vin', $_POST['vin']);
		$stmt->execute();
	}
	catch (Exception $e) {
		http_response_code(500);
		exit();
	}
?>
