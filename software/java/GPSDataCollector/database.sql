--
-- PostgreSQL database dump
--

SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = off;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET escape_string_warning = off;

SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: gpsdata; Type: TABLE; Schema: public; Owner: gpsdata; Tablespace: 
--

CREATE TABLE gpsdata (
    id integer NOT NULL,
    datetime timestamp without time zone NOT NULL,
    phonenumber character varying(30) NOT NULL,
    valid boolean DEFAULT false NOT NULL,
    latitude double precision NOT NULL,
    longitude double precision NOT NULL,
    speed double precision NOT NULL,
    heading double precision NOT NULL,
    signalstrength smallint DEFAULT 0 NOT NULL,
    imei bigint DEFAULT (-1) NOT NULL,
    satellites integer DEFAULT 0 NOT NULL,
    altitude double precision NOT NULL,
    batterystate smallint DEFAULT 0 NOT NULL,
    batteryvoltage double precision NOT NULL,
    charging boolean DEFAULT false NOT NULL,
    mcc integer NOT NULL,
    mnc integer NOT NULL,
    lac integer NOT NULL,
    cellid integer NOT NULL
);


ALTER TABLE public.gpsdata OWNER TO gpsdata;

--
-- Name: TABLE gpsdata; Type: COMMENT; Schema: public; Owner: gpsdata
--

COMMENT ON TABLE gpsdata IS 'GPS-Daten';


--
-- Name: gpsdata_id_seq; Type: SEQUENCE; Schema: public; Owner: gpsdata
--

CREATE SEQUENCE gpsdata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MAXVALUE
    NO MINVALUE
    CACHE 1;


ALTER TABLE public.gpsdata_id_seq OWNER TO gpsdata;

--
-- Name: gpsdata_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: gpsdata
--

ALTER SEQUENCE gpsdata_id_seq OWNED BY gpsdata.id;


--
-- Name: id; Type: DEFAULT; Schema: public; Owner: gpsdata
--

ALTER TABLE gpsdata ALTER COLUMN id SET DEFAULT nextval('gpsdata_id_seq'::regclass);


--
-- Name: gpsdata_pkey; Type: CONSTRAINT; Schema: public; Owner: gpsdata; Tablespace: 
--

ALTER TABLE ONLY gpsdata
    ADD CONSTRAINT gpsdata_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

