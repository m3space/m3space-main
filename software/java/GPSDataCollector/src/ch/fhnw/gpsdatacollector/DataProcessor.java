package ch.fhnw.gpsdatacollector;

public interface DataProcessor {
	
	
	void processGPSData(GPSDataSet data) throws DataProcessorException;

	void cleanUp();
}
