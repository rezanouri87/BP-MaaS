package customHandlers;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class PropertiesLoader {

	private static PropertiesLoader loader = new PropertiesLoader();
	private Properties prop;
	private long startListeningTime;
	public static final String START_EVENT_TYPE = "startEvent";
	public static final String TASK_EVENT_TYPE = "userTask";

	private PropertiesLoader() {
		startListeningTime = System.currentTimeMillis() + 60000;
	}

	public static PropertiesLoader getInstance() {
		return loader;
	}

	private void loadProperties() throws IOException {
		prop = new Properties();
		String propFileName = "config.properties";

		InputStream inputStream = getClass().getClassLoader().getResourceAsStream(propFileName);
		prop.load(inputStream);
		if (inputStream == null) {
			throw new FileNotFoundException("property file '" + propFileName + "' not found in the classpath");
		}
	}

	public String getPropertyValue(String key) {
		try {
			if (prop == null) {
				loadProperties();
			}
			return prop.getProperty(key);
		} catch (IOException e) {
			return "";
		}
	}

	public long getStartListeningTime() {
		return startListeningTime;
	}

	public void setStartListeningTime(long startListeningTime) {
		this.startListeningTime = startListeningTime;
	}

}
