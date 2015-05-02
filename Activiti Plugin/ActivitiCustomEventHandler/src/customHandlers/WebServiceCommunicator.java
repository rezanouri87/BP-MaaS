package customHandlers;

import javax.ws.rs.core.Response;

import customHandlers.wfccommunicator.EventEmitter;


public class WebServiceCommunicator {

	private static WebServiceCommunicator communicator = new WebServiceCommunicator();

	private WebServiceCommunicator() {
	}

	public static WebServiceCommunicator getInstance() {
		return communicator;
	}

	public void callWebservice(String req) {
		Response response = EventEmitter.logActivitiEvent(req);
		if (response.getStatus() == 500) {
			System.out
					.println("***************** Exception while calling WCF service*************");
		} else if (response.getStatus() == 200) {
			System.out
					.println("***************** Calling WCF service Succeeded*************");
		}

	}

}
