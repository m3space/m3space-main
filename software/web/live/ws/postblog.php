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
		$stmt = $dbh->prepare('INSERT INTO live_blog (utctimestamp, message) value (:utctimestamp, :message)');
		$stmt->bindParam(':utctimestamp', $_POST['utctimestamp']);
		$stmt->bindParam(':message', $_POST['message']);
		$stmt->execute();
	}
	catch (Exception $e) {
		http_response_code(500);
		exit();
	}
?>
