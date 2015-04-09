<?php
	require_once('../config.inc.php');
	
	if ($_SERVER['REQUEST_METHOD'] != 'POST') {
		http_response_code(405);
		exit();
	}
	
	$code = $_POST['code'];
	if ($code != $clearkey) {
		http_response_code(403);
		exit();
	}
	
	try {
		$dbh = new PDO("mysql:host=$dbhost;dbname=$dbname", $dbuser, $dbpass);
		
		$stmt = $dbh->prepare('DELETE FROM live_telemetry');
		$stmt->execute();
		
		$stmt = $dbh->prepare('DELETE FROM live_blog');
		$stmt->execute();
		
		$stmt = $dbh->prepare('DELETE FROM live_gpspos');
		$stmt->execute();
		
		$stmt = $dbh->prepare('DELETE FROM live_images');
		$stmt->execute();
		
		foreach (glob('../images/*.jpg') as $file) {
			unlink($file);
		}
		
		echo 'OK.';
	}
	catch (Exception $e) {
		http_response_code(500);
		exit();
	}
?>
