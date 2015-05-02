package customHandlers.wfccommunicator;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;

public class NetClientPost {
	 
	// http://localhost:8080/RESTfulExample/json/product/post
	public static void main(String[] args) {
		try {
			URL url = new URL("http://localhost:8080/RESTfulActivitiEngineLoggerWS/logger/logevent/");
			HttpURLConnection conn = (HttpURLConnection) url.openConnection();
			conn.setConnectTimeout(30000); // set timeout to 30 seconds
			conn.setRequestMethod("POST");
			conn.setRequestProperty("Content-Type", "text/plain");
			conn.setDoOutput(true);
			BufferedReader br = new BufferedReader(new InputStreamReader(
					(conn.getInputStream())));

			String line;
			StringBuffer output = new StringBuffer();
			while ((line = br.readLine()) != null) {
				output.append(line);
			}
			
			OutputStream os = conn.getOutputStream();
			String input = "<eventInfo><eventType>TASK_ASSIGNED</eventType><eventDescription>A task has been assigned to a user</eventDescription><eventDate>Tue Dec 30 19:44:40 AST 2014</eventDate><eventData><activityName>Approval task</activityName><activityType>userTask</activityType><processDefinition>simpleApprovalProcess</processDefinition><processInstanceId>1488</processInstanceId><assignee>kermit</assignee></eventData></eventInfo>";
			os.write(input.getBytes());
			os.flush();

			if (conn.getResponseCode() != 200) {
				throw new IOException("response code is "
						+ conn.getResponseCode());
			}
			conn.disconnect();
		} catch (IOException e) {
			System.out.println("*************** Exception while sending to WS"
					+ e.getMessage());
		}
 
	}
}
