<?php
	require_once('config.inc.php');
?>

<html>
<head>
	<title>Clear Data</title>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"> 
</head>
<body>
<h2>Clear all Data?</h2>
<form method="post" action="./ws/cleardata.php">
	<p>Enter Code:&nbsp;<input name="code" type="password" size="50" maxlength="50" /></p>
	<p><input name="submit" type="submit" value="Do it!" /></p>
</form>
</body>
</html>
