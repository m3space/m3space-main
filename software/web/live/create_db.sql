CREATE TABLE IF NOT EXISTS live_images (
  id int(11) NOT NULL AUTO_INCREMENT,
  utctimestamp datetime NOT NULL,
  filename varchar(30) NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;


CREATE TABLE IF NOT EXISTS live_telemetry (
  id int(11) NOT NULL AUTO_INCREMENT,
  utctimestamp datetime NOT NULL,
  latitude float NOT NULL,
  longitude float NOT NULL,
  galtitude float NOT NULL,
  paltitude float NOT NULL,
  heading smallint(6) NOT NULL,
  hspeed float NOT NULL,
  vspeed float NOT NULL,
  satellites tinyint(4) NOT NULL,
  inttemperature smallint(6) NOT NULL,
  temperature1 float NOT NULL,
  temperature2 float NOT NULL,
  pressure float NOT NULL,
  vin float NOT NULL,
  gamma int(11) NOT NULL,
  gammacpm float NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;


CREATE TABLE IF NOT EXISTS live_gpspos (
  id int(11) NOT NULL AUTO_INCREMENT,
  utctimestamp datetime NOT NULL,
  latitude float NOT NULL,
  longitude float NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;


CREATE TABLE IF NOT EXISTS live_blog (
  id int(11) NOT NULL AUTO_INCREMENT,
  utctimestamp datetime NOT NULL,
  message text NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;
