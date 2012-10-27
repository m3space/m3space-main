<?php
	require_once('config.inc.php');
?>

<html>
<head>
	<title>M3 Space Live Tracker</title>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"> 
	<link rel="stylesheet" type="text/css" href="main.css">
	<script src="https://maps.googleapis.com/maps/api/js?sensor=false"></script>
	<script src="jquery-1.7.2.min.js"></script>
	<script src="tracker.js"></script>
</head>
<body>
<div id="header">
	<h1>M3 Space Live Tracker</h1>
	<div id="message">Data is displayed for today, <?php echo date('Y-m-d'); ?></div>
</div>

<div id="telemetry">
	<div class="title">
	Latest Data
	</div>
	<table class="telemetry">
		<tr><td>UTC:</td><td><div id="t_utc"></div></td></tr>
		<tr><td>Latitude:</td><td><div id="t_lat"></div></td></tr>
		<tr><td>Longitude:</td><td><div id="t_lng"></div></td></tr>
		<tr><td>Altitude:</td><td><div id="t_galt"></div></td></tr>
		<tr><td>Heading:</td><td><div id="t_head"></div></td></tr>
		<tr><td>HSpeed:</td><td><div id="t_hspd"></div></td></tr>
		<tr><td>VSpeed:</td><td><div id="t_vspd"></div></td></tr>
		<tr><td>Sat.:</td><td><div id="t_sat"></div></td></tr>
		<tr><td>Int. Temp.:</td><td><div id="t_inttmp"></div></td></tr>
		<tr><td>Ext. Temp.:</td><td><div id="t_exttmp"></div></td></tr>
		<tr><td>Pressure:</td><td><div id="t_press"></div></td></tr>
		<tr><td>P. Altitude:</td><td><div id="t_palt"></div></td></tr>
		<tr><td>Vin:</td><td><div id="t_vin"></div></td></tr>
	</table>
	<div id="refreshbtn" class="button">Refresh</div>
</div>
<div id="map">
	<div class="title">
	Flight Map
	</div>
	<div id="mapcanvas"></div>
</div>
<div id="liveimage">
	<div class="title">
	Latest Live Image
	</div>
	<div class="liveimage">
	<img id="lastimage" class="live" src="" title="Live Image" alt="Image not available" />
	</div>
	<div id="lastimageupdate"></div>
</div>
<div id="footer">
&copy;2012 M3 Space
</div>
</body>
</html>
