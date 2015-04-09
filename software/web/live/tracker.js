var gmap;
var lastdata;
var lastBlog;
var endmarker;
var polyline;
var blog;
var gpspos;

function initializeBlog() {
	blog = [];
}

function initializeMap() {
	var mapOptions = {
		zoom: 12,
		center: new google.maps.LatLng(47.5, 7.8),
		mapTypeId: google.maps.MapTypeId.ROADMAP
	};
	gmap = new google.maps.Map(document.getElementById('mapcanvas'), mapOptions);
	
	var balloon = new google.maps.MarkerImage('balloon.png',
		new google.maps.Size(28, 42),
		new google.maps.Point(0,0),
		new google.maps.Point(14, 37));
	
	var car = new google.maps.MarkerImage('car.png',
		new google.maps.Size(44, 23),
		new google.maps.Point(0,0),
		new google.maps.Point(22, 15));
	
	var balloonOptions = {
		visible: false,
		map: gmap,
		icon: balloon
	}
	endmarker = new google.maps.Marker(balloonOptions);
	
	var carOptions = {
		visible: false,
		map: gmap,
		icon: car
	}
	carmarker = new google.maps.Marker(carOptions);
	
	var polyOptions = {
		strokeColor: '#000080',
		strokeOpacity: 0.5,
		strokeWeight: 2.0,
		visible: true,
		map: gmap
	};
	polyline = new google.maps.Polyline(polyOptions);

}      

function refresh() {
	var query = './ws/fetchdata.php';
	var params = []
	if (lastdata != null) {
		params.push('since=' + escape(lastdata));
	}
	if (lastBlog != null) {
		params.push('blogsince=' + escape(lastBlog));
	}
	if (params.length > 0) {
		query += '?';
	}
	for (var i = 0; i < params.length; i++) {
		query += params[i] + '&';
	}
	var msg = 'Last query: ' + new Date().toUTCString() + ' - ';
	$.getJSON(query)
	.success(function(data, status, jqXHR) {
	
		if (data.telemetry != null) {
			if (data.telemetry.length > 0) {
				msg += 'fetched ' + data.telemetry.length + ' data points';
			} else {
				msg += 'no data'
			}
			var last = null;
			$.each(data.telemetry, function() {
				last = this;
				var pos = new google.maps.LatLng(this.latitude, this.longitude);
				polyline.getPath().push(pos);
			});
			if (last != null) {
				lastdata = last.utctimestamp;
				var pos = new google.maps.LatLng(last.latitude, last.longitude);
				endmarker.setPosition(pos);
				endmarker.setVisible(true);
				gmap.setCenter(pos);
				
				$('#t_utc').html(last.utctimestamp);
				$('#t_lat').html(last.latitude + '&deg;');
				$('#t_lng').html(last.longitude + '&deg;');
				$('#t_galt').html(last.galtitude + ' m');
				$('#t_head').html(last.heading + '&deg;');
				$('#t_hspd').html(last.hspeed + ' m/s');
				$('#t_vspd').html(last.vspeed + ' m/s');
				$('#t_sat').html(last.satellites);
				$('#t_inttmp').html(last.inttemperature + '&deg;C');
				$('#t_t1').html(last.temperature1 + '&deg;C');
				$('#t_t2').html(last.temperature2 + '&deg;C');
				$('#t_press').html(last.pressure + ' bar');
				$('#t_palt').html(last.paltitude + ' m');
				$('#t_vin').html(last.vin + ' V');
				$('#t_gamma').html(last.gamma + ' C, ' + last.gammacpm + ' CPM');
			}
		}
		
		if (data.gpspos != null) {
			gpspos = data.gpspos;
			var pos = new google.maps.LatLng(gpspos.latitude, gpspos.longitude);
			carmarker.setPosition(pos);
			carmarker.setVisible(true);
		}
	
		if (data.liveimage != null) {
			$('#lastimage').attr('src', './images/' + data.liveimage.filename);
			$('#lastimageupdate').html(data.liveimage.utctimestamp + ' UTC');
		}

		if (data.blog != null) {
			var lastb = null;
			$.each(data.blog, function() {
				lastb = this;
				$('#blogitems').prepend('<tr><td><small>' + this.utctimestamp + ' UTC:</small><br />' + this.message + '<hr /></td></tr>');
			});
			if (lastb != null) {
				lastBlog = lastb.utctimestamp;
			}
		}
		$('#message').html(msg);
	})
	.error(function(jqXHR, status, error) {
		msg += 'data access error'
		$('#message').html(msg);
	});
}

$(document).ready(function() {
	lastupdate = null;

	initializeBlog();
	initializeMap();
	refresh();
	
	$("#refreshbtn").click(function() {
		refresh();
	});
	
	window.setInterval(refresh, 15000);
});
