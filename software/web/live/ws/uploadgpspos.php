<?php
	require_once('../config.inc.php');
	
	if ($_SERVER['REQUEST_METHOD'] != 'POST') {
		http_response_code(405);
		exit();
	}
	
	$key = $_POST['key'];
	if ($key != $authkey) {
		http_response_code(403);
		exit();
	}
	
	try {
		$dbh = new PDO("mysql:host=$dbhost;dbname=$dbname", $dbuser, $dbpass);
		$stmt = $dbh->prepare('INSERT INTO live_gpspos (utctimestamp, latitude, longitude) value (:utctimestamp, :latitude, :longitude)');
		$stmt->bindParam(':utctimestamp', $_POST['utctimestamp']);
		$stmt->bindParam(':latitude', $_POST['latitude']);
		$stmt->bindParam(':longitude', $_POST['longitude']);
		$stmt->execute();
	}
	catch (Exception $e) {
		http_response_code(500);
		exit();
	}
?>
